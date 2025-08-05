# Start from official nginx base image
FROM nginx:alpine

# Remove the default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy your custom nginx.conf into the container
COPY nginx.conf /etc/nginx/nginx.conf
