<div align="center">
    <img src="ReadmeImages/device_project_readme_banner_1500X450.png" alt="AudioShop Logo">
</div>

<div align="center">
    <h1 style="border-bottom: 0">DoorLockSystem</h1>
    <h3>Raspberry Pi 3 program</h3>
    <h4>Szakdolgozat</h4>
    <br>
</div>

---

<div>
    <h3>A szakdolgozat t�m�ja:</h3>
</div>

- Egy olyan eszk�z tervez�se, valamint egy olyan mobilalkalmaz�s k�sz�t�se, amely rendszerben m�k�dve k�pes kezelni szem�lyek be - �s kil�ptet�s�t.
- A szakdolgozat a k�t programon k�v�l tartalmaz m�g egy adatb�zist is, ami <b>t�rolt elj�r�sok</b> seg�ts�g�vel val�s�tja meg a k�t program <b>"Backend"</b> r�sz�t.

---

<div>
    <h3>A projekt le�r�sa:</h3>
</div>

- A program C# alap�, UWP-ben lett elk�sz�tve.
- Kompatibilit�si okok miatt Raspberry Pi 3-as verzi�j�ra lett meg�rva a program, emiatt esett a v�laszt�s a Microsoft UWP platformj�ra.

---

<div>
    <h3>Az eszk�z programj�nak grafikai megjelen�se:</h3>
</div>

- A log�t, valamint az alkalmaz�sban megtal�lhat� �sszes ikont az <b>Adobe Illustrator</b> nev� program seg�ts�g�vel alkottam meg.
- Az alkalmaz�s grafik�ja (log�t �s design-t egybev�ve) nem tartozik egyetlen val�s c�ghez sem, csak a szakdolgozat �rdek�ken k�sz�tettem el.

---

<div>
    <h3>A projekthez felhaszn�lt, C# alap� f�bb oszt�lyok, csomagok:</h3>
</div>

- Framework: UWP (Universal Windows Platform)
- A Raspberry Pi-hez sz�ks�ges csomagok:
    - Iot.Device.Bindings
    - System.Device.Gpio
    - System.IO.Ports


---

<div align="center">
    <br>
    <h3>A projekt r�szletes le�r�sa:</h3>
</div>

- A projekt 3 f� r�szb�l �ll:
    - Adatb�zisb�l, amely az adatokat szolg�ltatja az eszk�z �s a mobilalkalmaz�s sz�m�ra.
    - Az �ltalam elk�sz�tett eszk�zb�l, amely az �ltalam kiv�lasztott alkatr�szekb�l elk�sz�tettem.
    - Egy mobil alkalmaz�sb�l, amely a eszk�z�n t�rt�nt v�ltoz�sokat (be �s kil�p�seket) mutatja a bejelentkezett felhaszn�l�nak.
- <b>A tov�bbi r�sz az adatb�zisr�l, valamint az eszk�zr�l val� le�r�st tartalmazza.</b>

<div align="center">
    <br>
    <img src="ReadmeImages/devices_relationships.png" alt="AudioShop Logo">
    <p>A bel�ptet� rendszer r�szei</p>
</div>

---

<div align="center">
    <br>
    <h3>A program m�k�d�se:</h3>
</div>

<div align="center">
    <br>
    <img src="ReadmeImages/IMG_0338_rotated.gif" alt="AudioShop Logo">
    <p>A bel�ptet� rendszer r�szei</p>
</div>

