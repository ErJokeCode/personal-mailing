FROM node

ADD . /opt/client
WORKDIR /opt/client
RUN npm install

EXPOSE 5010

CMD ["npm", "run", "dev"]
