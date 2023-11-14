<?php

declare(strict_types=1);

namespace App\Utils;

use InvalidArgumentException;

use function App\Controllers\push;

class Response
{

    const MIN_STATUS_CODE_VALUE = 100;
    const MAX_STATUS_CODE_VALUE = 599;

    private $phrases = [
        // INFORMATIONAL CODES
        100 => 'Continue',
        101 => 'Switching Protocols',
        102 => 'Processing',
        103 => 'Early Hints',
        // SUCCESS CODES
        200 => 'OK',
        201 => 'Created',
        202 => 'Accepted',
        203 => 'Non-Authoritative Information',
        204 => 'No Content',
        205 => 'Reset Content',
        206 => 'Partial Content',
        207 => 'Multi-Status',
        208 => 'Already Reported',
        226 => 'IM Used',
        // REDIRECTION CODES
        300 => 'Multiple Choices',
        301 => 'Moved Permanently',
        302 => 'Found',
        303 => 'See Other',
        304 => 'Not Modified',
        305 => 'Use Proxy',
        306 => 'Switch Proxy', // Deprecated to 306 => '(Unused)'
        307 => 'Temporary Redirect',
        308 => 'Permanent Redirect',
        // CLIENT ERROR
        400 => 'Bad Request',
        401 => 'Unauthorized',
        402 => 'Payment Required',
        403 => 'Forbidden',
        404 => 'Not Found',
        405 => 'Method Not Allowed',
        406 => 'Not Acceptable',
        407 => 'Proxy Authentication Required',
        408 => 'Request Timeout',
        409 => 'Conflict',
        410 => 'Gone',
        411 => 'Length Required',
        412 => 'Precondition Failed',
        413 => 'Payload Too Large',
        414 => 'URI Too Long',
        415 => 'Unsupported Media Type',
        416 => 'Range Not Satisfiable',
        417 => 'Expectation Failed',
        418 => 'I\'m a teapot',
        421 => 'Misdirected Request',
        422 => 'Unprocessable Entity',
        423 => 'Locked',
        424 => 'Failed Dependency',
        425 => 'Unordered Collection',
        426 => 'Upgrade Required',
        428 => 'Precondition Required',
        429 => 'Too Many Requests',
        431 => 'Request Header Fields Too Large',
        444 => 'Connection Closed Without Response',
        451 => 'Unavailable For Legal Reasons',
        // SERVER ERROR
        499 => 'Client Closed Request',
        500 => 'Internal Server Error',
        501 => 'Not Implemented',
        502 => 'Bad Gateway',
        503 => 'Service Unavailable',
        504 => 'Gateway Timeout',
        505 => 'HTTP Version Not Supported',
        506 => 'Variant Also Negotiates',
        507 => 'Insufficient Storage',
        508 => 'Loop Detected',
        510 => 'Not Extended',
        511 => 'Network Authentication Required',
        599 => 'Network Connect Timeout Error',
    ];

    private $reasonPhrase = '';
    private $statusCode;
    private $errorCode;

    public function getPhrase($statusCode)
    {
        if (isset($this->phrases[$statusCode])) {
            return $this->phrases[$statusCode];
        }

        return null;
    }

    public function withArray(array $array)
    {
        $response = [
            'data' => $array
        ];

        return $this->jsonEncode($response);
    }

    public function withItem($item)
    {
        $response = [
            'data' => $item
        ];

        return $this->jsonEncode($response);
    }

    public function ok($message, $items = [])
    {
        $result = [
            'message' => $message
        ];
        $result = array_merge($result, $items);
        return $this->jsonEncode(
            [
                'result' => $result
            ]
        );
    }

    public function withError($message, int $statusCode, $items = [])
    {
        $error = [
            'http_code' => $statusCode,
            'phrase' => $this->getPhrase($statusCode),
            'message' => $message
        ];
        return $this->jsonEncode(
            [
                'error' => $error
            ]
        );
    }
    public function errorForbidden(string $message = '')
    {
        return $this->withError($message, 403);
    }

    public function errorInternalError(string $message = '')
    {
        return $this->withError($message, 500);
    }

    public function errorNotFound(string $message = '')
    {
        return $this->withError($message, 404);
    }

    public function errorUnauthorized(string $message = '')
    {
        return $this->withError($message, 401);
    }

    public function errorWrongArgs(array $message)
    {
        return $this->withError($message, 400);
    }

    public function errorGone(string $message = '')
    {
        return $this->withError($message, 410);
    }

    public function errorMethodNotAllowed(string $message = '')
    {
        return $this->withError($message, 405);
    }

    public function errorUnwillingToProcess(string $message = '')
    {
        return $this->withError($message, 431);
    }

    public function errorUnprocessable(string $message = '')
    {
        return $this->withError($message, 422);
    }

    public function jsonEncode($data)
    {
        if (is_resource($data)) {
            throw new InvalidArgumentException('Cannot JSON encode resources');
        }

        // Clear json_last_error()
        json_encode(null);

        $json = json_encode($data);

        if (JSON_ERROR_NONE !== json_last_error()) {
            throw new InvalidArgumentException(sprintf(
                'Unable to encode data to JSON in %s: %s',
                __CLASS__,
                json_last_error_msg()
            ));
        }

        return $json;
    }
}
