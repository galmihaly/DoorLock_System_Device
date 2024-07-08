<div align="center">
    <img src="FirstUwp/ReadmeImages/device_project_readme_banner_1500X450.png" alt="DoorLockSystem">
</div>

<div align="center">
    <h1 style="border-bottom: 0">DoorLockSystem</h1>
    <h3>Raspberry Pi 3 program</h3>
    <h4>Szakdolgozat</h4>
    <br>
</div>

---

<div>
    <h3>A szakdolgozat témája:</h3>
</div>

- Egy olyan eszköz tervezése, valamint egy olyan mobilalkalmazás készítése, amely rendszerben mûködve képes kezelni személyek be - és kiléptetését.
- A szakdolgozat a két programon kívül tartalmaz még egy adatbázist is, ami <b>tárolt eljárások</b> segítségével valósítja meg a két program <b>"Backend"</b> részét.

---

<div>
    <h3>A projekt leírása:</h3>
</div>

- A program C# alapú, UWP-ben lett elkészítve.
- Kompatibilitási okok miatt Raspberry Pi 3-as verziójára lett megírva a program, emiatt esett a választás a Microsoft UWP platformjára.

---

<div>
    <h3>Az eszköz programjának grafikai megjelenése:</h3>
</div>

- A logót, valamint az alkalmazásban megtalálható összes ikont az <b>Adobe Illustrator</b> nevû program segítségével alkottam meg.
- Az alkalmazás grafikája (logót és design-t egybevéve) nem tartozik egyetlen valós céghez sem, csak a szakdolgozat érdekéken készítettem el.

---

<div>
    <h3>A projekthez felhasznált, C# alapú fõbb osztályok, csomagok:</h3>
</div>

- Framework: UWP (Universal Windows Platform)
- A Raspberry Pi-hez szükséges csomagok:
    - Iot.Device.Bindings
    - System.Device.Gpio
    - System.IO.Ports


---

<div align="center">
    <br>
    <h3>A projekt részletes leírása:</h3>
</div>

- A projekt 3 fõ részbõl áll:
    - Adatbázisból, amely az adatokat szolgáltatja az eszköz és a mobilalkalmazás számára.
    - Az általam elkészített eszközbõl, amely az általam kiválasztott alkatrészekbõl elkészítettem.
    - Egy mobil alkalmazásból, amely a eszközön történt változásokat (be és kilépéseket) mutatja a bejelentkezett felhasználónak.
- <b>A további rész az adatbázisról, valamint az eszközrõl való leírást tartalmazza.</b>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/devices_relationships.png" alt="DoorLockSystem">
    <p>A beléptetõ rendszer részei</p>
</div>

---

<div align="center">
    <br>
    <h3>A program mûködése:</h3>
</div>

- A program 2 módszer szerint képes mûködni:
    - PIN kóddal: a felhasználó hozzá beregisztrált PIN kóddal képes belépni. 
    - RFID kártyával: a felhasználó hozzá beregisztrált RFID kártyájának használatával képes belépni.
- A módok közt a felhasználó a képernyõn újjával balra-jobbra való húzásával képes váltani.
- Az adatok hitelesítése közvetlen adatbázis kapcsolat során, tárolt eljárásban történik.

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/PIN_Code_Version.gif" alt="DoorLockSystem">
    <p>A beléptetõ rendszer PIN kódos módszere</p>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/RFID_Card_Version.gif" alt="DoorLockSystem">
    <p>A beléptetõ rendszer RFID kártyás módszere</p>
</div>

---

<div align="center">
    <br>
    <h3>Az eszköz felépítése képekben:</h3>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/device_cover_with_display.png" alt="DoorLockSystem">
    <p>Az eszköz külseje kijelzõvel</p>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/device_connectors.png" alt="DoorLockSystem">
    <p>Az eszköz csatlakozói</p>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/device_inside.png" alt="DoorLockSystem">
    <p>Az eszköz belseje: tápegység + csatlakozók kábelei</p>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/device_rfid_and_raspberry.png" alt="DoorLockSystem">
    <p>Az eszköz belseje: Raspberry Pi és RFID olvasó (piros színû)</p>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/device_sound_sensor.png" alt="DoorLockSystem">
    <p>Az eszköz belseje: hangszenzor</p>
</div>

<div align="center" style="transform: rotate(180deg);">
    <br>
    <img src="FirstUwp/ReadmeImages/PN532.png" alt="DoorLockSystem">
</div>

<div align="center">
    <p>PN532-es chippel rendelkezõ RFID olvasó</p>
</div>

<div align="center">
    <br>
    <img src="FirstUwp/ReadmeImages/RFID_SPI.jpg" alt="DoorLockSystem">
    <p>Raspberry Pi és RFID olvasó közti összeköttetés: SPI kapcsolat</p>
</div>