import uuid
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession

from app.models.category import Category, CategoryVersion
from app.models.version_types import ChangeType

async def create_category_version(
    session: AsyncSession,
    category: Category,
    change_type: ChangeType,
    tutor_id: uuid.UUID,
) -> CategoryVersion:
    """Создает запись версии категории"""
    category_version = CategoryVersion(
        category_id=category.id,
        name=category.name,
        tutor_id=tutor_id,
        change_type=change_type,
    )
    session.add(category_version)
    await session.flush()  # Получаем ID без коммита
    return category_version

async def get_category_versions(
    session: AsyncSession,
    skip: int,
    limit: int,
) -> list[CategoryVersion]:
    stmt = (
        select(CategoryVersion)
        .offset(skip)
        .limit(limit)
        .order_by(CategoryVersion.created_at.desc())
    )
    result = await session.execute(stmt)
    return list(result.scalars().all())

async def get_category_version(
    session: AsyncSession,
    version_id: uuid.UUID,
) -> CategoryVersion | None:
    stmt = select(CategoryVersion).where(CategoryVersion.id == version_id)
    result = await session.execute(stmt)
    return result.scalar_one_or_none()

async def apply_category_version(
    session: AsyncSession,
    version: CategoryVersion,
    tutor_id: uuid.UUID,
    category: Category,
) -> Category:
    """Применяет старую версию к текущей категории"""

    category.name = version.name


    await create_category_version(
        session=session,
        category=category,
        change_type=ChangeType.RESTORE,
        tutor_id=tutor_id,
    )

    await session.commit()
    await session.refresh(category)
    return category
