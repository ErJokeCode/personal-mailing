FROM node

ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN wget -qO- https://get.pnpm.io/install.sh | ENV="$HOME/.bashrc" SHELL="$(which bash)" bash -

WORKDIR /app

COPY package.json .
RUN pnpm install
COPY . .

EXPOSE 5015

CMD ["pnpm", "dev"]