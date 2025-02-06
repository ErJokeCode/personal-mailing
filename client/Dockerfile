FROM node

ADD . /client
WORKDIR /client
RUN npm install

EXPOSE 5010

CMD ["npm", "run", "dev"]
