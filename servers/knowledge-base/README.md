# Knowledge Base API - Инструкция для фронтенд-разработчика

## Обзор проекта

Knowledge Base (База знаний) - это система управления знаниями, разработанная для преподавателей IT-университета. Она позволяет создавать, редактировать, удалять и искать элементы знаний, которые включают в себя вопросы, ответы и теги для быстрого поиска.

## Технический стек

- **Backend**: FastAPI (Python)
- **База данных**: PostgreSQL
- **Поисковая система**: Elasticsearch
- **Хранилище файлов**: MinIO (S3-совместимое)
- **Инфраструктура**: Docker и docker-compose

## Базовый URL API

```
http://localhost:8000/api/v1
```

## Основные API эндпоинты

### Категории

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| GET | `/categories` | Получить все категории |
| GET | `/categories/{category_id}` | Получить категорию по ID |
| POST | `/categories` | Создать новую категорию |
| PATCH | `/categories/{category_id}` | Обновить категорию |
| DELETE | `/categories/{category_id}` | Удалить категорию |

#### Пример создания категории:
```json
POST /api/v1/categories
{
  "name": "Программирование на Python",
  "tutor_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Элементы знаний

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| GET | `/knowledge-items` | Получить все элементы знаний |
| GET | `/knowledge-items/{knowledge_item_id}` | Получить элемент знаний по ID |
| POST | `/knowledge-items` | Создать новый элемент знаний |
| PATCH | `/knowledge-items/{knowledge_item_id}` | Обновить элемент знаний |
| PATCH | `/knowledge-items/{knowledge_item_id}/tags` | Обновить теги элемента знаний |
| DELETE | `/knowledge-items/{knowledge_item_id}` | Удалить элемент знаний |

#### Пример создания элемента знаний:
```json
POST /api/v1/knowledge-items
{
  "question": "Как скомпилировать Go-приложение в исполняемый файл?",
  "answer": "Для компиляции Go-программы используйте команду `go build`.",
  "tutor_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "category_id": "4170c8da-d232-43cd-a96c-fa2ca2a6c8e4",
  "question_tags": [],
  "answer_tags": []
}
```

### Версионирование

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| GET | `/category-versions` | Получить все версии категорий |
| GET | `/category-versions/{category_id}` | Получить все версии конкретной категории |
| GET | `/knowledge-item-versions` | Получить все версии элементов знаний |
| GET | `/knowledge-item-versions/{knowledge_item_id}` | Получить все версии конкретного элемента знаний |

При любом изменении категории или элемента знаний автоматически создается новая версия с указанием типа изменения (создание, обновление, удаление).

### Поиск

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| POST | `/search` | Поиск по базе знаний |
| POST | `/search/reindex` | Переиндексировать все элементы знаний |

#### Пример поискового запроса:
```json
POST /api/v1/search
{
  "query": "python",
  "category_id": "4170c8da-d232-43cd-a96c-fa2ca2a6c8e4",
  "from_": 0,
  "size": 10
}
```

### Файлы

| Метод | Эндпоинт | Описание |
|-------|----------|----------|
| POST | `/files/upload` | Загрузить файл |
| GET | `/files/{object_name}` | Получить файл по имени |
| DELETE | `/files/{object_name}` | Удалить файл |

## Структура данных

### Категория

```json
{
  "id": "uuid",
  "name": "string",
  "tutor_id": "uuid",
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

### Элемент знаний

```json
{
  "id": "uuid",
  "question": "string",
  "answer": "string",
  "tutor_id": "uuid",
  "category_id": "uuid",
  "question_tags": ["string"],
  "answer_tags": ["string"],
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

### Версия категории

```json
{
  "id": "uuid",
  "category_id": "uuid",
  "name": "string",
  "tutor_id": "uuid",
  "change_type": "string", // CREATE, UPDATE, DELETE
  "created_at": "datetime"
}
```

### Версия элемента знаний

```json
{
  "id": "uuid",
  "knowledge_item_id": "uuid",
  "question": "string",
  "answer": "string",
  "category_id": "uuid",
  "tutor_id": "uuid",
  "question_tags": ["string"],
  "answer_tags": ["string"],
  "change_type": "string", // CREATE, UPDATE, DELETE
  "created_at": "datetime",
  "deleted_with_category_version_id": "uuid" // только если удалено вместе с категорией
}
```
