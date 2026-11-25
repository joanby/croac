# Panel de métricas de «Croac»

**Datos ficticios.** «Croac» no tiene jugadores: son los eventos del conjunto ficticio de la 04.6 del curso, 300 jugadores inventados durante 28 días, generados con una semilla fija (4) y con las columnas que escribe `AnaliticaLocal` (`t, usuario, sesion, evento, nivel, puntos, producto, ingreso`). El panel interactivo está en la lección 04.6.

## Métricas
| Métrica | Cómo se calcula | Valor |
|---|---|---|
| DAU medio | Jugadores distintos con algún evento cada día, media de los 28 días | 36,1 |
| Retención D1 | De los que instalan (su primer evento) un día, el % que vuelve al día siguiente | 31,7 % |
| Retención D7 | Lo mismo siete días después | 15,0 % |
| ARPU | Ingresos (compras y anuncios) / jugadores | 0,096 € |
| ARPDAU | Ingresos / suma de los DAU de todos los días | 0,0285 € |
| Conversión | % de jugadores con al menos una compra | 2,3 % |

## Embudo
| Paso | Jugadores |
|---|---|
| Juegan una partida | 300 |
| Juegan 5 partidas | 242 |
| Vuelven otro día | 242 |
| Ven un anuncio | 154 |
| Compran | 7 |

## Retención por día tras instalar
| Día | D0 | D1 | D2 | D3 | D4 | D5 | D6 | D7 | D8 | D9 | D10 | D11 | D12 | D13 | D14 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| % | 100,0 | 31,7 | 29,7 | 24,7 | 21,7 | 18,3 | 18,0 | 15,0 | 13,3 | 11,9 | 8,5 | 7,8 | 7,1 | 8,9 | 6,2 |

## La decisión
La de Marta, la misma del Lab 4: el escalón más caro es el primero. De 300 jugadores que juegan una partida, 242 llegan a cinco: uno de cada cinco juega una vez y no vuelve. Antes de tocar la tienda, la 1.1 mejora la primera partida, y el cambio se juzga por la retención D1.

## Ficheros
`metricas.csv`, `diario.csv`, `retencion.csv` y `embudo.csv`, copiados de la 04.6.
