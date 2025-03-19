FROM python:3.13-slim

ENV PYTHONUNBUFFERED 1
ENV PYTHONDONTWRITEBYTECODE 1

WORKDIR /knowledge-base

# Установка зависимостей и poetry проекта
RUN apt-get update && apt-get install -y --no-install-recommends \
    python3-dev \
    build-essential \
    && pip install --no-cache-dir --upgrade pip \
    && pip install --no-cache-dir poetry

# Копирование файлов зависимостей
COPY pyproject.toml poetry.lock* ./

# Установка зависимостей
RUN poetry config virtualenvs.create false \
    && poetry install --no-root --no-interaction --no-ansi

# Копирование всех файлов проекта
COPY ./app ./app

# Копирование миграций Alembic
COPY ./alembic ./alembic
COPY alembic.ini ./

# Копирование скрипта для наполнения базы тестовыми данными
COPY seed.py ./

EXPOSE 8000

# Запуск миграций и приложения
CMD alembic upgrade head && uvicorn app.main:app --host 0.0.0.0 --port 8000
