# build environment
FROM node as builder

WORKDIR /frontend

ENV PATH /frontend/node_modules/.bin:$PATH
COPY package.json /frontend/package.json
RUN npm install --silent && npm install react-scripts@1.1.1 --silent

COPY . .
RUN npm run build

# production environment
FROM nginx:alpine
COPY --from=builder /frontend/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

