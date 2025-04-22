from minio import Minio
from minio.error import S3Error
import logging
import traceback
from fastapi.concurrency import run_in_threadpool
from datetime import timedelta
import re

from app.core.config import settings

logger = logging.getLogger(__name__)

minio_client = Minio(
    settings.minio.endpoint,
    access_key=settings.minio.access_key,
    secret_key=settings.minio.secret_key,
    secure=settings.minio.secure
)


async def init_minio() -> None:
    """Инициализация MinIO: создание бакета если не существует"""
    try:
        if not minio_client.bucket_exists(settings.minio.bucket_name):
            minio_client.make_bucket(settings.minio.bucket_name)
            logger.info(f"Создан новый бакет MinIO: {settings.minio.bucket_name}")
        else:
            logger.info(f"Бакет MinIO уже существует: {settings.minio.bucket_name}")
    except S3Error as e:
        logger.error(f"Ошибка при инициализации MinIO: {e}")
        print(f"Ошибка при инициализации MinIO: {e}")


async def upload_file(file_path: str, object_name: str) -> str:
    """
    Загрузка файла в MinIO
    :param file_path: Путь к файлу
    :param object_name: Имя объекта в MinIO
    :return: URL файла
    """
    try:
        logger.info(f"Загрузка файла в MinIO: {object_name}")
        await run_in_threadpool(
            minio_client.fput_object,
            settings.minio.bucket_name,
            object_name,
            file_path
        )


        protocol = "https" if settings.minio.secure else "http"


        endpoint = settings.minio.endpoint
        if "minio:" in endpoint:
            endpoint = endpoint.replace("minio:", "localhost:")


        if ":" not in endpoint:
            endpoint = f"{endpoint}:9000"

        url = f"{protocol}://{endpoint}/{settings.minio.bucket_name}/{object_name}"
        logger.info(f"Файл успешно загружен: {url}")
        return url
    except S3Error as e:
        logger.error(f"Ошибка при загрузке файла {object_name}: {e}")
        print(f"Ошибка при загрузке файла {object_name}: {e}")
        traceback.print_exc()
        raise


async def download_file(object_name: str, file_path: str) -> None:
    """
    Скачивание файла из MinIO
    :param object_name: Имя объекта в MinIO
    :param file_path: Путь для сохранения файла
    """
    try:
        logger.info(f"Скачивание файла из MinIO: {object_name}")
        await run_in_threadpool(
            minio_client.fget_object,
            settings.minio.bucket_name,
            object_name,
            file_path
        )
        logger.info(f"Файл успешно скачан в {file_path}")
    except S3Error as e:
        logger.error(f"Ошибка при скачивании файла {object_name}: {e}")
        print(f"Ошибка при скачивании файла {object_name}: {e}")
        traceback.print_exc()
        raise


async def delete_file(object_name: str) -> None:
    """
    Удаление файла из MinIO
    :param object_name: Имя объекта в MinIO
    """
    try:
        logger.info(f"Удаление файла из MinIO: {object_name}")
        await run_in_threadpool(
            minio_client.remove_object,
            settings.minio.bucket_name,
            object_name
        )
        logger.info(f"Файл успешно удален: {object_name}")
    except S3Error as e:
        logger.error(f"Ошибка при удалении файла {object_name}: {e}")
        print(f"Ошибка при удалении файла {object_name}: {e}")
        traceback.print_exc()
        raise


async def get_file_url(object_name: str, expires: int = 7 * 24 * 60 * 60) -> str:
    """
    Получение временного URL для доступа к файлу в MinIO
    :param object_name: Имя объекта в MinIO
    :param expires: Время жизни URL в секундах (по умолчанию 7 дней)
    :return: Временный URL для доступа к файлу
    """
    try:
        logger.info(f"Запрос временного URL для файла: {object_name}")


        try:
            await run_in_threadpool(
                minio_client.stat_object,
                settings.minio.bucket_name,
                object_name
            )
        except Exception as stat_error:
            logger.error(f"Файл не найден в MinIO: {object_name} - {stat_error}")
            raise FileNotFoundError(f"Файл не найден: {object_name}")


        url = await run_in_threadpool(
            minio_client.presigned_get_object,
            settings.minio.bucket_name,
            object_name,
            timedelta(seconds=expires)
        )

        logger.info(f"Исходный URL из MinIO: {url}")


        if "minio:" in url:
            url = url.replace("minio:", "localhost:")


        url = re.sub(r'http://\d+\.\d+\.\d+\.\d+:', 'http://localhost:', url)


        if ".minio." in url:
            url = re.sub(r'http://.*\.minio\..*:', 'http://localhost:', url)


        endpoint_base = "http://localhost:9000"
        if "//" in url and not url.startswith("http://localhost:"):

            path_part = url.split('/', 3)[-1] if len(url.split('/', 3)) > 3 else ""
            url = f"{endpoint_base}/{path_part}"

        logger.info(f"Финальный исправленный URL: {url}")
        return url
    except Exception as e:
        logger.error(f"Ошибка при получении временного URL для файла {object_name}: {e}")
        print(f"❌ [GET_URL] Ошибка: {str(e)}")
        traceback.print_exc()
        raise


async def delete_file_from_minio(object_name: str) -> None:
    """
    Удаление файла из MinIO
    :param object_name: Имя объекта в MinIO
    """
    try:
        logger.info(f"Удаление файла из MinIO: {object_name}")


        try:
            await run_in_threadpool(
                minio_client.stat_object,
                settings.minio.bucket_name,
                object_name
            )
        except Exception as stat_error:
            logger.error(f"Файл не найден в MinIO: {object_name} - {stat_error}")
            raise FileNotFoundError(f"Файл не найден: {object_name}")

        await run_in_threadpool(
            minio_client.remove_object,
            settings.minio.bucket_name,
            object_name
        )
        logger.info(f"Файл успешно удален: {object_name}")
    except Exception as e:
        logger.error(f"Ошибка при удалении файла {object_name}: {e}")
        print(f"Ошибка при удалении файла {object_name}: {e}")
        traceback.print_exc()
        raise
