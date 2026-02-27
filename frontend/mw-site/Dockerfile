# Development Dockerfile for mw-site
FROM node:20-alpine

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Expose port
EXPOSE 4202

# Start development server with hot reload
CMD ["npm", "start", "--", "--host", "0.0.0.0", "--port", "4202"]
