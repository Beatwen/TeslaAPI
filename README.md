
# Tesla API Web Application

This web application provides an interface to interact with your Tesla vehicle via the Tesla API. With this app, you can remotely monitor your vehicle's status and execute various commands.




## Brief explanations

Upon launching the application, you'll be prompted to log in with your Tesla account. If you grant all access to the app, you won't be ask for login again.
After logging in, the app will display your vehicle's datas like battery level, state of the doors..
You will be able to choose favorites commands to choose from that will be displayed and saved for you to access in the home page.
Click on any of the predefined commands to execute them instantly.
The application will automatically refresh vehicle data every 20 seconds to ensure real-time updates.

## Code

In the code, the logic goes like this : 

- The first thing the server does is to get the partner token (in the local storage, if it's not found, it creates a new one)

- If there is no user token found in the local storage, the tesla login will prompt automatically. When finished with the login, the server will get the code from Tesla API to ask for a token for the user. It will save the response from tesla with the token AND the refresh token directly in the local storage. On next connexion, the server use the local storage to get the user token and check if it's still valid. If it's not, then it asks for a new one using the refresh token given by Tesla.

- The favorites commands of the user are then displayed (they're saved in the local storage too) They're all buttons that will trigger command to the api to post or get informations from the car.

- The server will check for the differents car of the user. It will display the ONLY first one if the user has more than 1 vehicle (For now, it will change in an update). With that information, it will perform all the commands for that car.

- Finally, we get all information from the car. The vehicle will be called every 20 seconds to check it's state and refresh the informations available on the screen. The call goes like this : asking from information from the vehicle, if we don't get it, we start to ask it to wake up. Whenever it wakes up, we get the information and display them.








## Sources of API

 - [Tesla API Docs](https://developer.tesla.com/docs/fleet-api)


## Authors

- [@Beatwen](https://www.github.com/beatwen)


## License

[MIT](https://choosealicense.com/licenses/mit/)


![Logo](https://upload.wikimedia.org/wikipedia/commons/e/e8/Tesla_logo.png)


