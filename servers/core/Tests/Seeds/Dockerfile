FROM mongo

ARG DATABASE_HOST
ENV DATABASE_HOST ${DATABASE_HOST}

COPY . .

CMD mongoimport --host ${DATABASE_HOST} --db personal-mailing --collection history --drop --file ./history.json && \
    mongoimport --host ${DATABASE_HOST} --db personal-mailing --collection info_online_course --drop --file ./info_online_course.json && \
    mongoimport --host ${DATABASE_HOST} --db personal-mailing --collection onl_cr_in_file --drop --file ./onl_cr_in_file.json && \
    mongoimport --host ${DATABASE_HOST} --db personal-mailing --collection student --drop --file ./student.json && \
    mongoimport --host ${DATABASE_HOST} --db personal-mailing --collection subject --drop --file ./subject.json 