from sqlalchemy import inspect, select
from sqlalchemy.engine import Result
from sqlalchemy.ext.asyncio import AsyncSession
import uuid
from typing import Optional, List

from app.models.category import Category
from app.schemas.category import CategoryCreate, CategoryUpdate
from app.models.version_types import ChangeType
from app.crud.category_version import create_category_version


async def get_categories(
    session: AsyncSession,
    skip: int = 0,
    limit: int = 100,
) -> List[Category]:
    stmt = select(Category).offset(skip).limit(limit)
    result = await session.execute(stmt)
    return list(result.scalars().all())


async def create_category(
    session: AsyncSession,
    name: str,
    tutor_id: uuid.UUID,
) -> Category:
    category = Category(name=name, tutor_id=tutor_id)
    session.add(category)
    await session.flush()

    await create_category_version(
        session=session,
        category=category,
        change_type=ChangeType.CREATE,
        tutor_id=tutor_id
    )

    await session.commit()
    await session.refresh(category)
    return category


async def get_category(
    session: AsyncSession,
    category_id: uuid.UUID,
) -> Optional[Category]:
    stmt = select(Category).where(Category.id == category_id)
    result = await session.execute(stmt)
    return result.scalar_one_or_none()


async def update_category(
    session: AsyncSession,
    category_id: uuid.UUID,
    name: str,
    tutor_id: uuid.UUID,
) -> Optional[Category]:
    category = await get_category(session, category_id)
    if not category:
        return None


    old_name = category.name


    old_version = await create_category_version(
        session=session,
        category=category,
        change_type=ChangeType.UPDATE,
        tutor_id=tutor_id
    )


    category.name = name

    await session.flush()

    await session.commit()
    await session.refresh(category)
    return category


async def delete_category(
    session: AsyncSession,
    category_id: uuid.UUID,
    tutor_id: uuid.UUID,
) -> bool:
    category = await get_category(session, category_id)
    if not category:
        return False


    category_data = {
        "category_id": category.id,
        "name": category.name,
        "tutor_id": tutor_id,
        "change_type": ChangeType.DELETE
    }


    await create_category_version(
        session=session,
        category=category,
        change_type=ChangeType.DELETE,
        tutor_id=tutor_id
    )

    await session.flush()

    # Удаляем категорию
    await session.delete(category)
    await session.commit()
    return True
