FROM python:3

ADD . /opt/parser
WORKDIR /opt/parser
RUN pip install -r requirements.txt

EXPOSE 8000

CMD ["python", "main.py"]
