FROM python:3

ADD . /parser
WORKDIR /parser
RUN pip install -r requirements.txt

EXPOSE 8000

CMD ["python", "main.py"]
