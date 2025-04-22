import os
import urllib.parse
from typing import Annotated, List, Optional
from uuid import uuid4
import logging
from fastapi.concurrency import run_in_threadpool

from fastapi import APIRouter, Depends, HTTPException, UploadFile, status, Response
from fastapi.responses import RedirectResponse
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.minio import get_file_url, upload_file, delete_file_from_minio, minio_client
from app.core.config import settings
from app.core.session import db_session
from app.schemas.file import FileResponse

logger = logging.getLogger(__name__)

router = APIRouter(tags=["files 📎"])


@router.post("/upload", response_model=FileResponse, status_code=status.HTTP_201_CREATED)
async def upload_image(
    file: UploadFile,
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
) -> FileResponse:
    """
    Загрузка изображения в MinIO.
    Поддерживаются форматы: jpg, jpeg, png, gif
    """
    try:
        content_type = file.content_type
        if not content_type or not content_type.startswith("image/"):
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="File must be an image",
            )

        temp_file_path = f"/tmp/{uuid4()}{os.path.splitext(file.filename)[1]}"
        try:
            with open(temp_file_path, "wb") as buffer:
                content = await file.read()
                buffer.write(content)

            object_name = f"images/{uuid4()}{os.path.splitext(file.filename)[1]}"
            url = await upload_file(temp_file_path, object_name)

            return FileResponse(url=url)
        finally:
            if os.path.exists(temp_file_path):
                os.remove(temp_file_path)
    except Exception as e:
        print(f"❌ [UPLOAD] Ошибка: {str(e)}")
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error uploading file: {str(e)}",
        )


@router.get("/{object_name:path}", response_class=Response)
async def get_file(object_name: str):
    """
    Получение файла из MinIO по имени объекта
    :param object_name: Имя объекта в MinIO
    :return: Файл из хранилища
    """
    try:

        decoded_name = urllib.parse.unquote(object_name)


        if decoded_name.startswith("knowledge-base/"):
            pure_object_name = decoded_name.replace("knowledge-base/", "", 1)
        else:
            pure_object_name = decoded_name


        content_type = None
        if pure_object_name.endswith(('.jpg', '.jpeg')):
            content_type = "image/jpeg"
        elif pure_object_name.endswith('.png'):
            content_type = "image/png"
        elif pure_object_name.endswith('.gif'):
            content_type = "image/gif"
        else:
            content_type = "application/octet-stream"


        try:

            response = await run_in_threadpool(
                minio_client.get_object,
                settings.minio.bucket_name,
                pure_object_name
            )


            content = await run_in_threadpool(response.read)


            await run_in_threadpool(response.close)


            return Response(
                content=content,
                media_type=content_type,
                headers={"Content-Disposition": f'inline; filename="{pure_object_name}"'}
            )
        except Exception as direct_error:
            logger.warning(f"Не удалось напрямую получить файл: {direct_error}")


            try:
                url = await get_file_url(pure_object_name)
                return RedirectResponse(url=url)
            except Exception as redirect_error:
                logger.error(f"Не удалось получить пресайн URL: {redirect_error}")
                raise HTTPException(
                    status_code=404,
                    detail=f"Файл {pure_object_name} не найден или недоступен"
                )
    except Exception as e:
        logger.error(f"Ошибка при получении файла {object_name}: {e}")
        raise HTTPException(
            status_code=500,
            detail=f"Ошибка сервера при получении файла: {str(e)}"
        )


@router.delete("/{object_name:path}", status_code=status.HTTP_204_NO_CONTENT)
async def delete_file(
    object_name: str,
    session: Annotated[AsyncSession, Depends(db_session.session_getter)],
) -> None:
    """
    Удаление файла из MinIO по имени.
    """
    try:

        decoded_name = urllib.parse.unquote(object_name)
        print(f"[DELETE] Исходный object_name: {object_name}")
        print(f"[DELETE] Декодированный object_name: {decoded_name}")


        if decoded_name.startswith("knowledge-base/"):
            pure_object_name = decoded_name.replace("knowledge-base/", "", 1)
        else:
            pure_object_name = decoded_name

        print(f"[DELETE] Удаляемый object_name: {pure_object_name}")


        await delete_file_from_minio(pure_object_name)
    except Exception as e:
        print(f"❌ [DELETE] Ошибка: {str(e)}")
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"File not found or cannot delete: {str(e)}",
        )
