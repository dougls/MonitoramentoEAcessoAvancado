var logger = require('./logger');

const logMiddleware = (req, res, next) => {
  res.on('finish', () => {
    const logLevel = res.statusCode >= 400 ? 'error' : 'info';
    logger.log(logLevel, `${res.statusCode} - ${req.method} ${req.originalUrl} - ${req.ip}`);
  });
  next();
};

module.exports = logMiddleware;