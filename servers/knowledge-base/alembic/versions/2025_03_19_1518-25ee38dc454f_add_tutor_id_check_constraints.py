"""add_tutor_id_check_constraints

Revision ID: 25ee38dc454f
Revises: c4891780158a
Create Date: 2025-03-19 15:18:59.393123

"""

from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa

# revision identifiers, used by Alembic.
revision: str = "25ee38dc454f"
down_revision: Union[str, None] = "c4891780158a"
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Add check constraints for tutor_id fields."""
    # Add check constraint for categories.tutor_id
    op.create_check_constraint(
        "check_tutor_id_positive",
        "categories",
        sa.column("tutor_id") > 0
    )

    # Add check constraint for knowledge_items.tutor_id
    op.create_check_constraint(
        "check_knowledge_item_tutor_id_positive",
        "knowledge_items",
        sa.column("tutor_id") > 0
    )


def downgrade() -> None:
    """Remove check constraints for tutor_id fields."""
    # Remove check constraint for knowledge_items.tutor_id
    op.drop_constraint(
        "check_knowledge_item_tutor_id_positive",
        "knowledge_items",
        type_="check"
    )

    # Remove check constraint for categories.tutor_id
    op.drop_constraint(
        "check_tutor_id_positive",
        "categories",
        type_="check"
    )
