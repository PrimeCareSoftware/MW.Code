# Build stage
FROM node:18-alpine AS build

WORKDIR /app

# Copy package files
COPY medicwarehouse-app/package*.json ./

# Install dependencies
RUN npm ci --only=production

# Copy source code
COPY medicwarehouse-app/ .

# Build the app
RUN npm run build --prod

# Production stage
FROM nginx:alpine

# Copy built app to nginx
COPY --from=build /app/dist/medicwarehouse-app/* /usr/share/nginx/html/

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port
EXPOSE 4200

CMD ["nginx", "-g", "daemon off;"]