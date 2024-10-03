const { createLogger, format, transports } = require('winston');

const logger = createLogger({
  level: 'debug',
  format: format.combine(
    format.timestamp(),
    format.json()
    // format.printf(({ timestamp, level, message, ...metadata }) => {
    //   let msg = `${timestamp} ${level}: ${message}`;
    //   if (metadata) {
    //     msg += ` ${JSON.stringify(metadata)}`;
    //   }
    //   return msg;
    // })
    // format.printf(({ timestamp, level, message }) => {
    //     return `${timestamp} ${level}: ${message}`;
    //   })
  ),
  transports: [
    new transports.Console(),
    new transports.File({ filename: 'combined.log' }), // Envia logs para um arquivo
    new transports.File({ filename: 'errors.log', level: 'error' }) // Envia apenas logs de nível 'error' para um arquivo específico
  ]
});

module.exports = logger;