# build environment
FROM node

WORKDIR /frontend
COPY . .
# ENV PATH /usr/src/app/node_modules/.bin:$PATH


COPY package.json /usr/src/app/package.json
RUN npm install --silent && npm install react-scripts@1.1.1 --silent

CMD ["npm", "run-script", "build"]

