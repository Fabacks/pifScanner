# pifScanner (English Version)

`pifScanner` is a simple, efficient desktop application for scanning your local network and listing all connected devices. The application is built on WPF and .NET, and is designed to be easily extendable.

## Features

- Local network scanning: Enter a range of IP addresses and the application will send a ping request to each address in that range to determine if a device is connected.
- Device details: For each connected device, the application displays the IP address, host name, MAC address, and response time.
- Parallel scanning: The application uses asynchronous programming to send ping requests in parallel, which speeds up the scanning process.
- Intuitive user interface: The user interface is straightforward and easy to use, with a button to start and stop scanning and a grid to display the results.

## How to use

1. Clone the repository using Git, or download the source code as a ZIP archive.
2. Open the project in Visual Studio.
3. Run the application using the "Start Debugging" command (F5).
4. In the application, enter the range of IP addresses you want to scan, then click the "Start scan" button.
5. The application will start scanning the network and displaying connected devices in the grid.

## Contributions

Contributions are welcome! If you have an idea for an improvement or if you've found a bug, feel free to open an issue or submit a pull request.

## License

`pifScanner` is licensed under the Creative Commons Attribution Non-Commercial (CC BY-NC) license. For more information, please see the [LICENSE](LICENSE) file in this repository.


---

# pifScanner

`pifScanner` est une application de bureau simple et efficace pour scanner votre réseau local et lister tous les appareils connectés. L'application est construite sur WPF et .NET, et est conçue pour être facilement extensible.

## Fonctionnalités

- Scanner le réseau local : Entrez une plage d'adresses IP et l'application va envoyer une requête ping à chaque adresse dans cette plage pour déterminer si un appareil est connecté.
- Détails des appareils : Pour chaque appareil connecté, l'application affiche l'adresse IP, le nom d'hôte, l'adresse MAC et le temps de réponse.
- Recherche en parallèle : L'application utilise la programmation asynchrone pour envoyer des requêtes ping en parallèle, ce qui accélère le processus de scan.
- Interface utilisateur intuitive : L'interface utilisateur est simple et facile à utiliser, avec un bouton pour démarrer et arrêter le scan, et une grille pour afficher les résultats.

## Comment l'utiliser

1. Clonez le dépôt à l'aide de Git, ou téléchargez le code source en tant qu'archive ZIP.
2. Ouvrez le projet dans Visual Studio.
3. Exécutez l'application à l'aide de la commande "Start Debugging" (F5).
4. Dans l'application, entrez la plage d'adresses IP que vous voulez scanner, puis cliquez sur le bouton "Lancer le scan".
5. L'application commencera à scanner le réseau et à afficher les appareils connectés dans la grille.

## Contributions

Les contributions sont les bienvenues ! Si vous avez une idée d'amélioration ou si vous avez trouvé un bug, n'hésitez pas à ouvrir une issue ou à soumettre une pull request.

## Licence

`pifScanner` est sous licence Creative Commons Attribution Non-Commercial (CC BY-NC). Pour plus d'informations, veuillez consulter le fichier [LICENSE](LICENSE) dans ce dépôt.