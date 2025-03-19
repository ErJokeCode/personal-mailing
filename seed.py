#!/usr/bin/env python3
"""
Скрипт для запуска загрузки тестовых данных в базу знаний.
Запускается как из корня проекта, так и внутри Docker-контейнера.
"""

import asyncio
import json
import logging
import os
import sys

from elasticsearch import ConnectionError as ESConnectionError, TransportError
from sqlalchemy.exc import SQLAlchemyError

from app.core.config import settings
from app.utils.seed_data import seed_data

# Определение корневой директории проекта
# В Docker контейнере скрипт находится в /knowledge-base/seed.py
# При локальном запуске - в текущей директории
if os.path.exists("/knowledge-base"):
    # Запуск в Docker
    project_root = "/knowledge-base"
else:
    # Локальный запуск
    project_root = os.path.abspath(os.path.dirname(__file__))

# Добавление директории проекта в PYTHONPATH
sys.path.insert(0, project_root)

# Настройка логирования
logging.basicConfig(
    level=settings.logging.log_level_value,
    format=settings.logging.log_format,
)
logger = logging.getLogger(__name__)


if __name__ == "__main__":
    logger.info(
        "Запуск загрузки тестовых данных (корневая директория: %s)...", project_root
    )
    try:
        asyncio.run(seed_data())
    except (SQLAlchemyError, ESConnectionError, TransportError) as e:
        logger.error("Ошибка при работе с базой данных или Elasticsearch: %s", e)
        sys.exit(1)
    except (FileNotFoundError, json.JSONDecodeError) as e:
        logger.error("Ошибка при работе с файлом данных: %s", e)
        sys.exit(1)
    except Exception as e:
        logger.error("Непредвиденная ошибка при загрузке данных: %s", e)
        sys.exit(1)
    logger.info("Загрузка тестовых данных завершена.")
