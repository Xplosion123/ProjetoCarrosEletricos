-- Nâo utilizar sem autorização!
drop database if exists db_loja_carros;
drop table if exists tb_usuario;
drop table if exists tb_carros;
drop table if exists tb_pagina_devs;

-- criação do banco de dados
create database if not exists db_loja_carros;

use db_loja_carros;

-- tabela de usuarios e funcionarios 
create table if not exists tb_usuario(
Id int primary key auto_increment,
Nome varchar(50) not null,
Email varchar (80) not null,
Senha varchar (150) not null,
Telefone varchar (15) default('n/a'),
Nivel varchar (20) default ('Usuario')
);

-- as categorias BEV e FCEV são dos carros totalmente elétricos
-- os outros 3 restantes são hubridos
create table if not exists tb_carros(
Id_carro int primary key auto_increment,
Nome_carro varchar(100) not null,
Marca enum (
'BYD',
'Tesla',
'GWM',
'Geely',
'Chery',
'GAC Aion',
'Leapmotor',
'Jaecoo',
'Toyota',
'Volvo',
'Renault',
'Hyundai',
'Kia',
'BMW',
'Mercedes-Benz',
'Chevrolet',
'Fiat',
'Ford',
'Volkswagen'
),
Descricao TEXT,
Imagem varchar(250),
Categoria enum('BEV','FCEV','PHEV','HEV','MHEV') not null,
Preco decimal (10,2) not null,
Data_cadastro datetime default current_timestamp
);

-- tabela para a pagina em php
-- os nomes dos dev servem para catalogar uma nota de satisfação do usuario
create table if not exists tb_pagina_devs(
Id int auto_increment primary key,
Nome_usuario varchar (100) not null,
Email_usuario varchar(100) not null,
Henrique_n int default ('Não teve nota'),
Igor_n int default ('Não teve nota'),
Bruno_n int default ('Não teve nota')
);


-- criando a tabela de dados mortos
drop table if exists tb_usuario_apagado;

create table tb_usuario_apagado(
    Id int,
    Nome varchar(50),
    Email varchar(80),
    Senha varchar(150),
    Telefone varchar(15) default ('n/a'),
    Nivel varchar(20),
    Data_apagada datetime default current_timestamp
);

-- criando o trigger para fazer a função de enviar as informações do usuario deletado
-- para a tabela morta
delimiter //

create trigger trg_arquivar_usuario
before delete on tb_usuario
for each row
begin

    insert into tb_usuario_apagado
    (Id, Nome, Email, Senha, Telefone, Nivel)

    values
    (
        OLD.Id,
        OLD.Nome,
        OLD.Email,
        OLD.Senha,
        OLD.Telefone,
        OLD.Nivel
    );

end //
delimiter ;


-- acessos para os admins
insert into tb_usuario (Nome, Email, Senha, Nivel)
values ('adimin','adm@email.com','$2a$11$xa4QYUU.pOHnXtSHss/kRuT.iDy3g6L62YGhpPZUCy3lkqxJ.lZ0O','Admin');

insert into tb_usuario (Nome, Email, Senha, Nivel)
values ('devB','oneadm@email.com','@@..', 'Admin');


select*from tb_usuario;
select*from tb_usuario_apagado;

