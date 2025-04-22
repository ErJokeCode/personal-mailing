import asyncio
import uuid
from typing import Any, Dict

from sqlalchemy import text
from sqlalchemy.ext.asyncio import AsyncSession

from app.core.config import settings
from app.core.session import db_session


async def convert_table_to_uuid(
    session: AsyncSession,
    table_name: str,
    id_column: str = "id",
    foreign_key_columns: list[str] | None = None,
) -> None:
    # Создаем временную таблицу с UUID
    temp_table_name = f"{table_name}_temp"
    await session.execute(
        text(
            f"""
            CREATE TABLE {temp_table_name} (LIKE {table_name} INCLUDING ALL);
            ALTER TABLE {temp_table_name} ALTER COLUMN {id_column} TYPE uuid USING gen_random_uuid();
            """
        )
    )

    # Если есть внешние ключи, добавляем их в новую таблицу
    if foreign_key_columns:
        for fk_column in foreign_key_columns:
            await session.execute(
                text(
                    f"""
                    ALTER TABLE {temp_table_name} ALTER COLUMN {fk_column} TYPE uuid USING gen_random_uuid();
                    """
                )
            )

    # Копируем данные из старой таблицы в новую
    await session.execute(
        text(
            f"""
            INSERT INTO {temp_table_name} SELECT * FROM {table_name};
            """
        )
    )

    # Удаляем старую таблицу и переименовываем новую
    await session.execute(
        text(
            f"""
            DROP TABLE {table_name};
            ALTER TABLE {temp_table_name} RENAME TO {table_name};
            """
        )
    )

    await session.commit()


async def main() -> None:
    async with db_session.session_factory() as session:
        # Конвертируем таблицы в порядке их зависимостей
        await convert_table_to_uuid(
            session,
            "categories",
            foreign_key_columns=["tutor_id"],
        )
        await convert_table_to_uuid(
            session,
            "knowledge_items",
            foreign_key_columns=["tutor_id", "category_id"],
        )


if __name__ == "__main__":
    asyncio.run(main()) 