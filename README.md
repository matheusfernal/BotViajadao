# Bot Viajadão
Projeto final da Maratona Bots da Microsoft. Um bot que dá recomendações para viajantes.

Para executar o bot localmente, será necessário criar um arquivo *Debug.AppSettings.config* na pasta AppSettings. 
Copie o conteúdo do arquivo *Release.AppSettings.config* para ele substituindo os tokens #{}# pelos valores das chaves dos serviços utilizados.
Para rodar em localhost o BotId, MicrosoftAppId e MicrosoftAppPassword podem ser deixados em branco.

No arquivo *Release.AppSettings.config* utilizo tokens #{}# para substituir pelas chaves durante o release que é feito no VSTS.
Para fazer o release manual ou pelo próprio Visual Studio, pode-se utilizar as chaves diretamente nesse arquivo. 

Só lembre-se de nunca subir as chaves para repositórios públicos ;)
