# База знаний для тьюторов IT-вуза 🎓

![FastAPI](https://img.shields.io/badge/FastAPI-0.115-009485?style=flat&logo=fastapi&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Latest-336791?style=flat&logo=postgresql&logoColor=white)
![Elasticsearch](https://img.shields.io/badge/Elasticsearch-8.17.3-00bfb3?style=flat&logo=elasticsearch&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Latest-2496ED?style=flat&logo=docker&logoColor=white)
![Python](https://img.shields.io/badge/Python-3.13-3776AB?style=flat&logo=python&logoColor=white)

Система управления базой знаний для тьюторов IT-вуза, позволяющая эффективно хранить, классифицировать и искать решения распространенных проблем.

## Возможности системы 🚀

- 📝 **Управление знаниями**: создание, чтение, обновление и удаление элементов знаний
- 🗂️ **Категоризация**: группировка элементов знаний по категориям
- 🔍 **Полнотекстовый поиск**: быстрый поиск по всей базе знаний с помощью Elasticsearch
- 👥 **Идентификация тьюторов**: каждая запись содержит ID тьютора, который ее создал
- 🚀 **REST API**: доступ ко всем функциям через современный FastAPI-интерфейс
- 📊 **Пагинация**: эффективная работа с большими наборами данных
- 🔄 **Асинхронность**: высокая производительность благодаря асинхронным операциям

## Технологический стек 🛠️

### Backend
- **FastAPI**
- **Pydantic**
- **SQLAlchemy**
- **Alembic**

### База данных и поиск
- **PostgreSQL**:
- **Elasticsearch**

### Контейнеризация
- **Docker**: Контейнеризация приложения
- **Docker Compose**: Оркестрация контейнеров

1. **Настройка переменных окружения**
   ```bash
   # Основные переменные
   cp .env.template .env

   # Переменные приложения
   cp app/.env.template app/.env
   ```

   Настройте `.env`:
   ```ini
   # PostgreSQL
   POSTGRES_USER=postgres
   POSTGRES_PASSWORD=changeme
   POSTGRES_DATABASE=database

   # Elasticsearch
   ELASTIC_USERNAME=elastic
   ELASTIC_PASSWORD=changeme
   ELASTIC_VERIFY_CERTS=False
   ```

   Настройте `app/.env`:
   ```ini
   # База данных
   APP_CONFIG__DB__URL=postgresql+asyncpg://postgres:changeme@postgres:5432/database
   APP_CONFIG__DB__ECHO=1

   # Elasticsearch
   APP_CONFIG__ES__URL=http://elasticsearch:9200
   APP_CONFIG__ES__USER=elastic
   APP_CONFIG__ES__PASSWORD=changeme
   APP_CONFIG__ES__VERIFY_CERTS=False
   ```

2. **Проверка работоспособности**

   Откройте в браузере:
   - 🌐 API: http://localhost:8000
   - 📚 Swagger UI: http://localhost:8000/docs
   - 📖 ReDoc: http://localhost:8000/redoc

### Использование Makefile

Проект содержит Makefile для упрощения часто используемых команд. Это позволяет выполнять сложные операции с помощью коротких команд.

#### Таблица доступных команд Makefile

| Команда | Описание | Действие |
|---------|----------|----------|
| `make up` | Запуск всех контейнеров в фоновом режиме | `docker-compose up -d` |
| `make up-fg` | Запуск всех контейнеров с выводом логов в терминал | `docker-compose up` |
| `make down` | Остановка и удаления всех контейнеров, сетей и томов, определенных в вашем файле docker-compose.yml| `docker-compose down` |
| `make restart` | Перезапуск всех контейнеров | `docker-compose restart` |
| `make logs` | Просмотр логов | `docker-compose logs -f` |
| `make ps` | Просмотр статуса контейнеров | `docker-compose ps` |
| `make seed` | Наполнение базы тестовыми данными (в Docker) | `docker-compose exec app python /knowledge-base/seed.py` |
| `make seed-local` | Наполнение базы тестовыми данными (локально) | `python seed.py` |
| `make migrate` | Применение миграций | `docker-compose exec app alembic upgrade head` |
| `make migration` | Создание новой миграции | `docker-compose exec app alembic revision --autogenerate -m "$(message)"` |
| `make db-reset` | Сброс и пересоздание базы данных (Linux/WSL) | Сложная команда из нескольких шагов |
| `make db-reset-win` | Сброс и пересоздание базы данных (Windows) | То же, что и db-reset, но в Windows-совместимом формате |
| `make clean` | Очистка данных и удаление контейнеров (Linux/WSL) | `docker-compose down; docker system prune -f; sudo rm -rf postgres-data elasticsearch-data` |
| `make clean-win` | Очистка данных и удаление контейнеров (Windows) | `docker-compose down; docker system prune -f; rmdir /s /q postgres-data elasticsearch-data` |
| `make stop-all` | Остановка всех Docker контейнеров в системе (Linux/WSL) | `docker stop $(docker ps -aq)` |
| `make stop-all-win` | Остановка всех Docker контейнеров в системе (Windows) | Остановка всех контейнеров с помощью цикла for |
| `make prune-all` | Полная очистка системы Docker | `docker system prune -a --volumes -f` |
| `make help` | Справка по доступным командам | Выводит описание всех команд |

#### Примеры использования команд

1. **Основные команды**
   ```bash
   # Запуск всех контейнеров
   make up

   # Остановка всех контейнеров
   make down

   # Перезапуск всех контейнеров
   make restart

   # Просмотр логов
   make logs

   # Просмотр статуса контейнеров
   make ps
   ```

2. **Работа с базой данных**
   ```bash
   # Применение миграций
   make migrate

   # Создание новой миграции
   make migration message="описание изменений"

   # Сброс и пересоздание базы данных (Linux/WSL)
   make db-reset

   # Сброс и пересоздание базы данных (Windows)
   make db-reset-win
   ```

> 📝 **Информация о миграциях**: Миграции базы данных запускаются автоматически при старте контейнера. Ручной запуск с помощью `make migrate` требуется только в следующих случаях:
> - После ручного сброса базы данных
> - При возникновении ошибок в автоматической миграции
> - При разработке новых миграций для их тестирования

**Когда может потребоваться ручное применение миграций:**
- После ручной очистки данных или сброса базы данных
- После выполнения команд `make clean`/`make clean-win` или `make db-reset`/`make db-reset-win`
- При тестировании новых миграций

```bash
# Типичная последовательность команд при первом запуске
make up         # запустить контейнеры (миграции запустятся автоматически)
make seed       # (опционально) заполнить базу тестовыми данными
```

3. **Наполнение тестовыми данными**
   ```bash
   # В Docker-контейнере
   make seed
   ```

4. **Управление Docker и очистка системы**
   ```bash
   # Остановка всех контейнеров Docker в системе (Linux/WSL)
   make stop-all

   # Остановка всех контейнеров Docker в системе (Windows)
   make stop-all-win

   # Полная очистка системы Docker
   make prune-all

   # Очистка данных проекта и удаление контейнеров (Linux/WSL)
   make clean

   # Очистка данных проекта и удаление контейнеров (Windows)
   make clean-win

   # Сброс и пересоздание базы данных (Linux/WSL)
   make db-reset

   # Сброс и пересоздание базы данных (Windows)
   make db-reset-win
   ```

> ⚠️ **Примечание для пользователей Windows**: Для команд `clean`, `stop-all` и `db-reset` созданы специальные Windows-версии с суффиксом `-win`. Используйте их вместо стандартных версий, которые содержат Linux-специфичный синтаксис и будут работать только в WSL.

## Устранение проблем 🔧

### Проблемы с Docker

1. **Контейнеры не запускаются**
   ```bash
   # Остановка всех Docker контейнеров в системе
   make stop-all

   # Полная очистка системы Docker
   make prune-all

   # Удаление данных
   sudo rm -rf postgres-data elasticsearch-data

   # Пересборка
   docker-compose up -d --build
   ```

   Или с использованием Makefile:
   ```bash
   # Остановка всех контейнеров
   make stop-all

   # Очистка системы и пересборка
   make prune-all
   make clean
   make up
   ```

2. **Мониторинг и диагностика**
   ```bash
   # Логи конкретного сервиса
   docker-compose logs -f app
   docker-compose logs -f postgres
   docker-compose logs -f elasticsearch

   # Статус контейнеров
   docker-compose ps

   # Использование ресурсов
   docker stats
   ```

## API Endpoints 🔌

Все эндпоинты доступны по базовому пути `/api/v1`

### Категории 📚

Категории используются для группировки элементов знаний по темам.

| Метод | Эндпоинт | Описание | Параметры запроса | Тело запроса | Ответ |
|-------|----------|----------|------------------|--------------|-------|
| `GET` | `/categories` | Получение списка всех категорий | `skip`: начальная позиция (по умолчанию 0)<br>`limit`: максимальное количество (по умолчанию 100) | - | Массив объектов категорий |
| `POST` | `/categories` | Создание новой категории | - | `name`: название категории (5-150 символов)<br>`tutor_id`: ID тьютора | Созданная категория |
| `GET` | `/categories/{category_id}` | Получение категории по ID | `category_id`: ID категории | - | Объект категории |
| `PUT` | `/categories/{category_id}` | Полное обновление категории | `category_id`: ID категории | `name`: название категории<br>`tutor_id`: ID тьютора | Обновленная категория |
| `PATCH` | `/categories/{category_id}` | Частичное обновление категории | `category_id`: ID категории | `name`: название категории (опционально)<br>`tutor_id`: ID тьютора (опционально) | Обновленная категория |
| `DELETE` | `/categories/{category_id}` | Удаление категории | `category_id`: ID категории | - | - |

#### Пример объекта категории:
```json
{
  "id": 1,
  "name": "Python",
  "tutor_id": 1,
  "created_at": "2023-03-15T14:30:00.000Z",
  "updated_at": "2023-03-15T14:30:00.000Z"
}
```

### Элементы знаний 📖

Элементы знаний содержат вопросы и ответы, которые помогают тьюторам решать распространенные проблемы.

| Метод | Эндпоинт | Описание | Параметры запроса | Тело запроса | Ответ |
|-------|----------|----------|------------------|--------------|-------|
| `GET` | `/knowledge-items` | Получение списка всех элементов знаний | `skip`: начальная позиция (по умолчанию 0)<br>`limit`: максимальное количество (по умолчанию 100) | - | Массив объектов элементов знаний |
| `POST` | `/knowledge-items` | Создание нового элемента знаний | - | `question`: вопрос<br>`answer`: ответ<br>`tutor_id`: ID тьютора<br>`category_id`: ID категории | Созданный элемент знаний |
| `GET` | `/knowledge-items/{knowledge_item_id}` | Получение элемента знаний по ID | `knowledge_item_id`: ID элемента знаний | - | Объект элемента знаний |
| `PUT` | `/knowledge-items/{knowledge_item_id}` | Полное обновление элемента знаний | `knowledge_item_id`: ID элемента знаний | `question`: вопрос<br>`answer`: ответ<br>`tutor_id`: ID тьютора<br>`category_id`: ID категории | Обновленный элемент знаний |
| `PATCH` | `/knowledge-items/{knowledge_item_id}` | Частичное обновление элемента знаний | `knowledge_item_id`: ID элемента знаний | `question`: вопрос (опционально)<br>`answer`: ответ (опционально)<br>`tutor_id`: ID тьютора (опционально)<br>`category_id`: ID категории (опционально) | Обновленный элемент знаний |
| `DELETE` | `/knowledge-items/{knowledge_item_id}` | Удаление элемента знаний | `knowledge_item_id`: ID элемента знаний | - | - |

#### Пример объекта элемента знаний:
```json
{
  "id": 1,
  "question": "Как установить Python?",
  "answer": "Скачайте установщик с официального сайта python.org и следуйте инструкциям.",
  "tutor_id": 1,
  "category_id": 1,
  "created_at": "2023-03-15T15:00:00.000Z",
  "updated_at": "2023-03-15T15:00:00.000Z"
}
```

### Поиск 🔍

Поиск позволяет находить элементы знаний по ключевым словам с использованием Elasticsearch.

| Метод | Эндпоинт | Описание | Параметры запроса | Тело запроса | Ответ |
|-------|----------|----------|------------------|--------------|-------|
| `POST` | `/search` | Поиск элементов знаний | - | `query`: поисковый запрос<br>`category_id`: ID категории для фильтрации (опционально)<br>`tutor_id`: ID тьютора для фильтрации (опционально)<br>`from_`: начальная позиция для пагинации (по умолчанию 0)<br>`size`: размер страницы (по умолчанию 10, макс. 100) | Результаты поиска |

#### Пример запроса поиска:
```json
{
  "query": "python установка",
  "category_id": 1,
  "from_": 0,
  "size": 10
}
```

#### Пример ответа поиска:
```json
{
  "total": 1,
  "hits": [
    {
      "id": 1,
      "question": "Как установить Python?",
      "answer": "Скачайте установщик с официального сайта python.org и следуйте инструкциям.",
      "tutor_id": 1,
      "category_id": 1,
      "created_at": "2023-03-15T15:00:00.000Z",
      "updated_at": "2023-03-15T15:00:00.000Z",
      "score": 0.9876543,
      "highlight": {
        "question": ["Как <em>установить</em> <em>Python</em>?"],
        "answer": ["Скачайте <em>установщик</em> с официального сайта <em>python</em>.org и следуйте инструкциям."]
      }
    }
  ],
  "took": 5
}
```

## Docker-конфигурация 🐳

Проект использует Docker Compose для управления контейнерами. Основные сервисы:

### app
- Основное приложение FastAPI
- Зависит от сервисов postgres и elasticsearch
- Порт: 8000
- Монтирует директорию ./app для разработки

### postgres
- База данных PostgreSQL
- Порт: 5432
- Переменные окружения:
  - POSTGRES_USER
  - POSTGRES_PASSWORD
  - POSTGRES_DB
- Данные сохраняются в ./postgres-data

### elasticsearch
- Elasticsearch 8.17.3
- Порт: 9200
- Настройки:
  - Одноузловой кластер (discovery.type=single-node)
  - Безопасность включена (xpack.security.enabled=true)
  - Ограничение памяти: 512MB
  - Уровень логирования: ERROR
  - Логи ограничены: max-size=10m, max-file=3
- Данные не сохраняются в постоянный volume (закомментировано)

### Сеть
- Все сервисы используют сеть knowledge-base-network (bridge)

### Переменные окружения
- Все сервисы используют переменные из файла .env
- Для переменных установлены значения по умолчанию

### Проверка здоровья
- Для postgres и elasticsearch настроены healthcheck
- Приложение запускается только после успешной проверки зависимых сервисов
