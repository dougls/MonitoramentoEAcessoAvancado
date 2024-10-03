FROM node:18-slim

RUN mkdir -p /home/node/app/node_modules 

WORKDIR /home/node/app

COPY package*.json ./

RUN npm install

# COPY datadog.yaml ./

ENV DD_API_KEY="add-api-key-here"
ENV DD_APP_KEY="add-app-key-here"
ENV DD_AGENT_MAJOR_VERSION=7
ENV DD_SITE="us5.datadoghq.com"
ENV DD_ENV=production
ENV DD_HOSTNAME="shark-node-logs"
ENV DD_LOGS_INJECTION=true
ENV DD_LOGS_ENABLED=true
ENV DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL=true
ENV DD_CONTAINER_EXCLUDE="name:datadog-agent"

RUN apt-get update && \
    apt-get install -y curl gnupg && \
    bash -c "$(curl -L https://s3.amazonaws.com/dd-agent/scripts/install_script_agent7.sh)" && \
    mv /etc/datadog-agent/security-agent.yaml.example /etc/datadog-agent/security-agent.yaml
    # mv ./datadog.yaml /etc/datadog-agent/datadog.yaml

COPY . .

EXPOSE 3002

CMD ["sh", "-c", "service datadog-agent start && node app.js"]