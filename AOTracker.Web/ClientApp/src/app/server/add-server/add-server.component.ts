import { Component, OnInit, Input, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-add-server',
  templateUrl: './add-server.component.html',
  styleUrls: ['./add-server.component.css']
})
export class AddServerComponent implements OnInit {
  public name: string
  public webUrl: string
  public usersEndpoint: string
  public serverErrorMessage: string;
  public serverSuccessMessage: string;
  public serverSavingMessage: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.name = '';
    this.webUrl = '';
    this.usersEndpoint = '';
  }

  ngOnInit() {
  }

  public OnServerSave(): any {
    var server = { name: this.name, webUrl: this.webUrl, usersendpoint: this.usersEndpoint };

    this.serverErrorMessage = null;
    this.serverSavingMessage = "Guardando...";
    this.serverSuccessMessage = null;

    console.log(server);
    this.http.post(this.baseUrl + "servers", server)
      .subscribe((r: PostServerResponse) => {
        console.log(r);
        this.serverSavingMessage = null;

        if (!r || !r.hasError) {
          this.serverSuccessMessage = "Guardando... Hecho! (Puede tomar unos minutos en empezar a registrar datos)"
          return;
        }

        this.serverErrorMessage = "No pudimos agregar el servidor debido a: <br> <ul>";

        if (r.isNameRepeated)
          this.serverErrorMessage += "<li>Ya hay un servidor con ese nombre.</li>";

        if (r.isUsersEndpointRepeated)
          this.serverErrorMessage += "<li>Ya hay un servidor con ese endpoint de usuarios.</li>";

        if (r.isWebRepeated)
          this.serverErrorMessage += "<li>Ya hay un servidor con la misma Web.</li>";

        if (r.usersEndpointIsNotValid)
          this.serverErrorMessage += "<li>No se obtuvo una respuesta válida del endpoint de usuarios.</li>";

        if (r.webIsNotValid)
          this.serverErrorMessage += "<li>No se obtuvo una respuesta válida al intentar checkear la Web provista.</li>";

        this.serverErrorMessage += "</ul>";
      });
  }
}


interface PostServerResponse {
  isNameRepeated: boolean;
  isWebRepeated: boolean;
  isUsersEndpointRepeated: boolean;
  hasError: boolean;
  serverData: any;
  webIsNotValid: boolean;
  usersEndpointIsNotValid: boolean;
}
