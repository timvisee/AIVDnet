using UnityEngine;
using System.Collections;

public class Agent {

    private string id;
    private string name;
    private string code;
    private string birthdateDay;
    private string birthdateMonth;
    private string birthdateYear;
    private string streetName;
    private string streetNumber;
    private string postCode;
    private string city;
    private string swimCert;

    public Agent(string data) {
        // Split the data
        string[] parts = data.Split(';');

        // Set the agent data
        id = parts[0];
        name = parts[1];
        code = parts[2];
        birthdateDay = parts[3];
        birthdateMonth = parts[4];
        birthdateYear = parts[5];
        streetName = parts[6];
        streetNumber = parts[7];
        postCode = parts[8];
        city = parts[9];
        swimCert = parts[10];
    }

    public string GetId() {
        return id;
    }

    public string GetName() {
        return name;
    }

    public string GetCode() {
        return code;
    }

    public string GetBirthday() {
        return birthdateDay + " " + birthdateMonth + " " + birthdateYear;
    }

    public string GetStreet() {
        return streetName + " " + streetNumber;
    }

    public string GetPostalCode()  {
        return postCode + " " + city;
    }

    public string GetSwimCert() {
        return swimCert;
    }
}
