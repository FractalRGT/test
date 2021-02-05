Исполняемый файл в bin/debug

Команды:
SET key value
value может быть любой: строка, список, словарь
список просто элементы через пробел
SET key value1 value2 value3 ...
словарь также парами через пробел
SET key value11 value12 value21 value22 value31 value32 ...
В конце строки можно написать EX и время в миллисекундах чтобы поставить TTL
SET key value EX 10000
DEL key удалит ключ
GET key вернёт значение по ключу
LGET и HGET тоже самое для списков и словарей
KEYS pattern вернёт все ключи содержащие pattern в названии