const tracer = require('dd-trace').init()
const logger = require('./logger');
const logMiddleware = require('./middleware.js');
const express = require("express");
var app = express();
var router = express.Router();

var path = __dirname + '/views/';
const PORT = 3002;
const HOST = '0.0.0.0';

router.use(logMiddleware);
app.use(logMiddleware);

// router.use((req,res,next) => {
//   const isValid = false // router.get("/sharks/1");
//   if (!isValid) {
//     res.status(404);
//     logger.error(`404 - Not Found - ${req.originalUrl} - ${req.method} - ${req.ip}`)
  
//   } else {
    
//     res.status(200);
//     logger.info(`Received request: ${req.method} ${req.url} from ${req.ip}`);
//   }
//   next();
// });

router.use((req,res,next) => {
  res.on('finish', () => {
    const logLevel = res.statusCode >= 400 ? 'error' : 'info';
    logger.log(logLevel, `${res.statusCode} - ${req.method} ${req.url} - ${req.ip}`);
  
    const span = tracer.scope().active();
    if (span) {
      span.setTag('error', true);
      span.setTag('http.status_code', res.statusCode);
      span.setTag('resource.name', req.originalUrl);
      span.setTag('span.kind', 'server');
      span.setTag('http.method', req.method);
      span.setTag('http.url', req.originalUrl);
      span.setTag('http.client_ip', req.ip);
    }

    // Verificar se a resposta já foi enviada
    if (!res.headersSent) {
      res.status(res.statusCode).json({
        status: res.statusCode,
        error: res.statusCode === 404 ? 'Not Found' : 'Internal Server Error',
        message: err.message
      });
    }
  });
  next();
});

router.get("/",function(req,res){
  res.sendFile(path + "index.html");
  res.status(200).send(`200 - OK - ${req.originalUrl} - ${req.method} - ${req.ip}`);
});

router.get("/sharks",function(req,res){
  res.sendFile(path + "sharks.html");
  res.status(200).send(`200 - OK - ${req.originalUrl} - ${req.method} - ${req.ip}`);
});

router.use((req, res, next) => {
  res.status(404).json({
    status: 404,
    error: 'Not Found'
  });

  const error = new Error('Not Found');
  error.status = 404;
  next(error);
});

router.use((err, req, res, next) => {
  const statusCode = err.status || 500;

  // Registrar o erro
  logger.error(`${statusCode} - ${err.message} - ${req.originalUrl} - ${req.method} - ${req.ip}`);

  // Configurar o Datadog APM para tratar o 404 como erro
  const span = tracer.scope().active();
  if (span) {
    span.setTag('error', true);
    span.setTag('http.status_code', statusCode);
    span.setTag('resource.name', req.originalUrl);
    span.setTag('span.kind', 'server');
    span.setTag('http.method', req.method);
    span.setTag('http.url', req.originalUrl);
    span.setTag('http.client_ip', req.ip);
  }

  // Verificar se a resposta já foi enviada
  if (!res.headersSent) {
    res.status(statusCode).json({
      status: statusCode,
      error: statusCode === 404 ? 'Not Found' : 'Internal Server Error',
      message: err.message
    });
  }
});

app.use(express.static(path));
app.use("/", router);

app.use((req, res, next) => {
  res.status(404).json({
    status: 404,
    error: 'Not Found'
  });

  const error = new Error('Not Found');
  error.status = 404;
  next(error);
});

app.use((err, req, res, next) => {
  const statusCode = err.status || 500;

  // Registrar o erro
  logger.error(`${statusCode} - ${err.message} - ${req.originalUrl} - ${req.method} - ${req.ip}`);

  // Configurar o Datadog APM para tratar o 404 como erro
  const span = tracer.scope().active();
  if (span) {
    span.setTag('error', true);
    span.setTag('http.status_code', statusCode);
    span.setTag('resource.name', req.originalUrl);
    span.setTag('span.kind', 'server');
    span.setTag('http.method', req.method);
    span.setTag('http.url', req.originalUrl);
    span.setTag('http.client_ip', req.ip);
  }

  // Verificar se a resposta já foi enviada
  if (!res.headersSent) {
    res.status(statusCode).json({
      status: statusCode,
      error: statusCode === 404 ? 'Not Found' : 'Internal Server Error',
      message: err.message
    });
  }
});

app.listen(PORT, function () {
  logger.info(`Shark Node listening on port ${PORT}!`);
});