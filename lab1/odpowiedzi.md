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

Kąt: 30 stopni

```
[-F+F+F][+F-F-F]
```
