from sqlalchemy import inspect, select
from sqlalchemy.engine import Result
from sqlalchemy.ext.asyncio import AsyncSession

from app.models.category import Category
from app.schemas.category import CategoryCreate, CategoryUpdate


async def get_categories(
    session: AsyncSession,
    skip: int,
    limit: int,
) -> list[Category]:
    stmt = select(Category).offset(skip).limit(limit).order_by(Category.id)
    result: Result = await session.execute(stmt)
    return list(result.scalars().all())


async def create_category(
    category_in: CategoryCreate,
    session: AsyncSession,
) -> Category:
    category = Category(**category_in.model_dump())
    session.add(category)
    await session.commit()
    await session.refresh(category)
    return category


async def get_category_by_id(
    category_id: int,
    session: AsyncSession,
) -> Category | None:
    stmt = select(Category).where(Category.id == category_id)
    result: Result = await session.execute(stmt)
    return result.scalar_one_or_none()


async def update_category(
    category_id: int,
    category_in: CategoryUpdate,
    session: AsyncSession,
    exclude_unset: bool,
) -> Category | None:
    category = await get_category_by_id(category_id, session)
    if not category:
        return None

    update_data = category_in.model_dump(exclude_unset=exclude_unset)

    # Получаем информацию о колонках модели
    mapper = inspect(Category)
    non_nullable_columns = [
        c.key for c in mapper.columns if not c.nullable and c.key != "id"
    ]

    # Проверка tutor_id = 0
    if "tutor_id" in update_data and update_data["tutor_id"] == 0:
        raise ValueError("tutor_id не может быть равен 0")

    # Для обязательных полей с NULL значениями:
    for field in non_nullable_columns:
        if field in update_data and update_data[field] is None:
            if exclude_unset:
                # Для PATCH - пропускаем NULL значения
                del update_data[field]
            else:
                # Для PUT - выдаем ошибку, т.к. обязательные поля не могут быть NULL
                raise ValueError(
                    f"Поле '{field}' не может быть NULL при полном обновлении (PUT)"
                )

    for field, value in update_data.items():
        setattr(category, field, value)

    await session.commit()
    await session.refresh(category)
    return category


async def delete_category(
    category_id: int,
    session: AsyncSession,
) -> bool:
    category = await get_category_by_id(category_id, session)
    if not category:
        return False

    await session.delete(category)
    await session.commit()
    return True
