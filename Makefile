.PHONY: up up-fg down restart logs ps seed seed-local stop-all stop-all-win prune-all clean clean-win db-reset db-reset-win

# Запуск всех контейнеров в фоновом режиме
up:
	docker-compose up -d

# Запуск всех контейнеров с выводом логов в терминал
up-fg:
	docker-compose up

# Остановка всех контейнеров
down:
	docker-compose down

# Перезапуск всех контейнеров
restart:
	docker-compose restart

# Просмотр логов
logs:
	docker-compose logs -f

# Просмотр статуса контейнеров
ps:
	docker-compose ps

# Наполнение базы тестовыми данными (в Docker)
seed:
	docker-compose exec app python /knowledge-base/seed.py

# Наполнение базы тестовыми данными (локально)
seed-local:
	python seed.py

# Применение миграций
migrate:
	docker-compose exec app alembic upgrade head

# Создание новой миграции
migration:
	docker-compose exec app alembic revision --autogenerate -m "$(message)"

# Сброс и пересоздание базы данных (Linux/WSL)
db-reset:
	docker-compose exec postgres dropdb -U postgres knowledge-base || true
	docker-compose exec postgres createdb -U postgres knowledge-base
	$(MAKE) migrate
	$(MAKE) seed

# Сброс и пересоздание базы данных (Windows)
db-reset-win:
	docker-compose exec postgres dropdb -U postgres knowledge-base || true
	docker-compose exec postgres createdb -U postgres knowledge-base
	$(MAKE) migrate
	$(MAKE) seed

# Очистка данных (Linux/WSL)
clean:
	docker-compose down
	docker system prune -f
	sudo rm -rf postgres-data elasticsearch-data

# Очистка данных для Windows (без sudo)
clean-win:
	docker-compose down
	docker system prune -f
	-rmdir /s /q postgres-data elasticsearch-data

# Остановка всех контейнеров Docker в системе (Linux/WSL)
stop-all:
	docker stop $$(docker ps -aq)

# Остановка всех контейнеров Docker в системе (Windows)
stop-all-win:
	@for /f "tokens=*" %i in ('docker ps -aq') do @docker stop %i

# Полная очистка системы Docker (образы, контейнеры, тома)
prune-all:
	docker system prune -a --volumes -f

# Справка
help:
	@echo "Доступные команды:"
	@echo "  make up              - Запуск всех контейнеров в фоновом режиме"
	@echo "  make up-fg           - Запуск всех контейнеров с выводом логов в терминал"
	@echo "  make down            - Остановка всех контейнеров"
	@echo "  make restart         - Перезапуск всех контейнеров"
	@echo "  make logs            - Просмотр логов"
	@echo "  make ps              - Просмотр статуса контейнеров"
	@echo "  make seed            - Наполнение базы тестовыми данными (в Docker)"
	@echo "  make seed-local      - Наполнение базы тестовыми данными (локально)"
	@echo "  make migrate         - Применение миграций"
	@echo "  make migration       - Создание новой миграции (message='описание')"
	@echo "  make db-reset        - Сброс и пересоздание базы данных (Linux/WSL)"
	@echo "  make db-reset-win    - Сброс и пересоздание базы данных (Windows)"
	@echo "  make clean           - Очистка данных (Linux/WSL)"
	@echo "  make clean-win       - Очистка данных (для Windows CMD/PowerShell)"
	@echo "  make stop-all        - Остановка всех контейнеров Docker в системе (Linux/WSL)"
	@echo "  make stop-all-win    - Остановка всех контейнеров Docker в системе (Windows)"
	@echo "  make prune-all       - Полная очистка системы Docker (образы, контейнеры, тома)"
	@echo "  make help            - Справка" 