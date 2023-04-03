# Generátor Platobných Predpisov

## O aplikácii
Aplikácia slúži na generovanie platobných predpisov pre väčšie množstvo platiteľov.
Každý vygenerovaný predpis obsahuje informácie o platbe a QR kód na jednoduché zaplatenie.

Vstupom je vyplnený platobný hárok - OpenXml Spreadsheet dokument vytvorený podľa šablóny 
[platobny_harok.xltx](GeneratorPP/wwwroot/platobny_harok.xltx).

Výstupom je dokument s platobnými predpismi - OpenXml Spreadsheet dokument vytvorený podľa šablóny 
[platobne_predpisy.xltx](GeneratorPP/wwwroot/platobne_predpisy.xltx).

## Licencia
Licencia poskytnutá podľa [EUPL 1.2](https://joinup.ec.europa.eu/sites/default/files/inline-files/EUPL%20v1_2%20SK.txt)

## Dokumentácia
QR kódy sú vytvárané podľa "Pay by square" [štandardu](http://www.sbaonline.sk/sk/projekty/qr-platby/) Slovenskej Bankovej Asociácie.

- Schéma dátového modelu ([/Documentation/bysquare-pay-1.1.0.xsd](GeneratorPP/Documentation/bysquare-pay-1.1.0.xsd))
    - online [dokumentácia](https://www.bsqr.co/schema/)
- Špecifikácia "Pay by square" štandardu ([/Documentation/bysquare-payspecifications-1.1.0.pdf](GeneratorPP/Documentation/bysquare-payspecifications-1.1.0.pdf))
- Špecifikácia API webovej služby app.bysquare.com ([/Documentation/bySquare_API_SVK.pdf](GeneratorPP/Documentation/bySquare_API_SVK.pdf))
- Logo manuál ([/Documentation/bysquare-logo-manual-1.0.4.pdf](GeneratorPP/Documentation/bysquare-logo-manual-1.0.4.pdf))

## Použité diela
- "Pay by square" špecifikácia (Slovenská Banková Asociácia) - [licencia](http://www.sbaonline.sk/sk/projekty/qr-platby/podmienky-pouzitia-specifikacia-standardu-pay-square.html)
- [DocumenFormat.OpenXml](https://www.nuget.org/packages/DocumentFormat.OpenXml/) (Microsoft) -	licencia nebola špecifikovaná
- [QRCoder](https://www.nuget.org/packages/QRCoder/) (Raffael Herrmann) - [licencia](https://github.com/codebude/QRCoder/blob/master/LICENSE.txt) MIT
- [Base32 Encoder and Decoder in C#](http://scottless.com/blog/archive/2014/02/15/base32-encoder-and-decoder-in-c.aspx) (Oleg Ignat) - [licencia](https://creativecommons.org/licenses/by/2.0/) CC BY 2.0
- [SharpCompress](https://www.nuget.org/packages/sharpcompress/) (adamhathcock) - [licencia](https://github.com/adamhathcock/sharpcompress/blob/master/LICENSE.txt) MIT
- [SixLabors.ImageSharp](https://www.nuget.org/packages/SixLabors.ImageSharp/) (sixlabors) - [licencia](http://www.apache.org/licenses/LICENSE-2.0) Apache 2.0
- [SixLabors.ImageSharp.Drawing](https://www.nuget.org/packages/SixLabors.ImageSharp.Drawing/) (sixlabors) - [licencia](http://www.apache.org/licenses/LICENSE-2.0) Apache 2.0

