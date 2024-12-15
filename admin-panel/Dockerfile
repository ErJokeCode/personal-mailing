FROM node

ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"

ADD . /opt/admin-panel
WORKDIR /opt/admin-panel

RUN wget -qO- https://get.pnpm.io/install.sh | ENV="$HOME/.bashrc" SHELL="$(which bash)" bash -
RUN pnpm install

EXPOSE 5015

CMD ["pnpm", "dev"]
