FROM python:3

ADD . /opt/bot
WORKDIR /opt/bot
RUN pip install -r requirements.txt

EXPOSE 8000

CMD ["python", "main.py"]
