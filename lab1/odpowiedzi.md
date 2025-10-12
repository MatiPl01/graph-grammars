# L-systemy

**1. Proszę w programie na stronie www.alife.pl zdefiniować L-system rysujący krzywą Kocha**

Kąt: 60 stopni

```
F
F: F+F--F+F
```

**2. Proszę w programie na stronie www.alife.pl zdefiniować L-system rysujący następujące drzewo:**

Kąt: 45 stopni

```
F
F: F[+F][-F[-F]F]F[-F][+F]
```

**3. Proszę w programie na stronie www.alife.pl zdefiniować L-system rysujący następujące drzewo:**

Kąt: 20 stopni

```
F[+F][-F[-F]F]F[+F][-F]
F: F[+F][-F[-F]F]F[-F][+F]
```

**4. Proszę w programie na stronie www.alife.pl zdefiniować L-system rysujący następujące drzewo (rozrastają się tylko niektóre „górne gałęzie”):**

Kąt: 30 stopni

```
F[+F][-F[-F]F]F[+FA][-FA]
A: F[+F][-F[-F]F]F[+FA][-FA]
```

**5. Proszę zdefiniować regułę przepisującą generującą liść**

Kąt: 30 stopni

```
[-F+F+F][+F-F-F]
```

**6. Proszę do drzewa wygenerowanego w poprzednim zadaniu dodać generację liści.**

Nie byłem w stanie dostać tego samego, ale wygląda i tak dobrze.

Kąt: 25 stopni

```
Y
Y: Y[+Y][-Y[-Y]Y]T
T: B[+B][-B[-B]BL]B[-B][+BL]B
B: FFF
L: [-F+F+F][+F-F-F]
```

**7. Proszę wygenerować własne ciekawe drzewo (z liśćmi lub kwiatami, korzeniami, itd...)**

Kąt: 20 stopni
Rekursja: 7
Długość kroku: 8

```
[---------R]BY
Y: Y[+Y+Y][-Y]T
T: B[-BL]B[+BL]B[[L]+B+ffA]
B: FFF
L: [-F+F+F][+F-F-F]
R: [-R][+R]FR
A: +L+L+L+L+L+L+L+L+L+L+L+L+L+L+L+L+L+L
```

**8. Proszę wygenerować krzywą Hilberta**

```
X
X: -YF+XFX+FY-
Y: +XF-YFY-FX+
```

**9. Proszę na końce krzywej dodać liście (kwadraty, bo tu kąt 90)**

```
[+L]XL
X: -YF+XFX+FY-
Y: +XF-YFY-FX+
L: [F+F+F+F]
```
