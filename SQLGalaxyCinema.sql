CREATE DATABASE TestGalaxyCinema;
GO
USE TestGalaxyCinema;
GO
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('HASACTOR') and o.name = 'FK_HASACTOR_HASACTOR_ACTOR')
alter table HASACTOR
   drop constraint FK_HASACTOR_HASACTOR_ACTOR
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('HASACTOR') and o.name = 'FK_HASACTOR_HASACTOR2_MOVIE')
alter table HASACTOR
   drop constraint FK_HASACTOR_HASACTOR2_MOVIE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('HASGENRE') and o.name = 'FK_HASGENRE_HASGENRE_MOVIE')
alter table HASGENRE
   drop constraint FK_HASGENRE_HASGENRE_MOVIE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('HASGENRE') and o.name = 'FK_HASGENRE_HASGENRE2_GENRE')
alter table HASGENRE
   drop constraint FK_HASGENRE_HASGENRE2_GENRE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MOVIE') and o.name = 'FK_MOVIE_DIRECTEDB_DIRECTOR')
alter table MOVIE
   drop constraint FK_MOVIE_DIRECTEDB_DIRECTOR
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MOVIE') and o.name = 'FK_MOVIE_HASAGERAT_AGERATIN')
alter table MOVIE
   drop constraint FK_MOVIE_HASAGERAT_AGERATIN
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('MOVIE') and o.name = 'FK_MOVIE_PRODUCEDI_NATION')
alter table MOVIE
   drop constraint FK_MOVIE_PRODUCEDI_NATION
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('REVIEW') and o.name = 'FK_REVIEW_HASREVIEW_MOVIE')
alter table REVIEW
   drop constraint FK_REVIEW_HASREVIEW_MOVIE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('REVIEW') and o.name = 'FK_REVIEW_WRITES_CUSTOMER')
alter table REVIEW
   drop constraint FK_REVIEW_WRITES_CUSTOMER
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('SEAT') and o.name = 'FK_SEAT_CONTAINSS_THEATER')
alter table SEAT
   drop constraint FK_SEAT_CONTAINSS_THEATER
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('SHOWTIME') and o.name = 'FK_SHOWTIME_HOSTS_THEATER')
alter table SHOWTIME
   drop constraint FK_SHOWTIME_HOSTS_THEATER
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('SHOWTIME') and o.name = 'FK_SHOWTIME_ISSHOWNIN_MOVIE')
alter table SHOWTIME
   drop constraint FK_SHOWTIME_ISSHOWNIN_MOVIE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKET') and o.name = 'FK_TICKET_HASTICKET_SHOWTIME')
alter table TICKET
   drop constraint FK_TICKET_HASTICKET_SHOWTIME
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKET') and o.name = 'FK_TICKET_ISPAIDBY_PAYMENT')
alter table TICKET
   drop constraint FK_TICKET_ISPAIDBY_PAYMENT
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKET') and o.name = 'FK_TICKET_PURCHASES_CUSTOMER')
alter table TICKET
   drop constraint FK_TICKET_PURCHASES_CUSTOMER
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKET') and o.name = 'FK_TICKET_SELLS_EMPLOYEE')
alter table TICKET
   drop constraint FK_TICKET_SELLS_EMPLOYEE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKETDETAIL') and o.name = 'FK_TICKETDE_HASPRICE_TICKETPR')
alter table TICKETDETAIL
   drop constraint FK_TICKETDE_HASPRICE_TICKETPR
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKETDETAIL') and o.name = 'FK_TICKETDE_INCLUDESD_TICKET')
alter table TICKETDETAIL
   drop constraint FK_TICKETDE_INCLUDESD_TICKET
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKETPRICE') and o.name = 'FK_TICKETPR_APPLIESTO_DAYCAT')
alter table TICKETPRICE
   drop constraint FK_TICKETPR_APPLIESTO_DAYCAT
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKETPRICE') and o.name = 'FK_TICKETPR_APPLIESTO_TICKETTY')
alter table TICKETPRICE
   drop constraint FK_TICKETPR_APPLIESTO_TICKETTY
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKETSEAT') and o.name = 'FK_TICKETSE_ASSIGNS_TICKETDE')
alter table TICKETSEAT
   drop constraint FK_TICKETSE_ASSIGNS_TICKETDE
go
if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TICKETSEAT') and o.name = 'FK_TICKETSE_HASSEAT_SEAT')
alter table TICKETSEAT
   drop constraint FK_TICKETSE_HASSEAT_SEAT
go
if exists (select 1
            from  sysobjects
           where  id = object_id('ACTOR')
            and   type = 'U')
   drop table ACTOR
go
if exists (select 1
            from  sysobjects
           where  id = object_id('AGERATING')
            and   type = 'U')
   drop table AGERATING
go
if exists (select 1
            from  sysobjects
           where  id = object_id('CUSTOMER')
            and   type = 'U')
   drop table CUSTOMER
go
if exists (select 1
            from  sysobjects
           where  id = object_id('DAYCAT')
            and   type = 'U')
   drop table DAYCAT
go
if exists (select 1
            from  sysobjects
           where  id = object_id('DIRECTOR')
            and   type = 'U')
   drop table DIRECTOR
go
if exists (select 1
            from  sysobjects
           where  id = object_id('EMPLOYEE')
            and   type = 'U')
   drop table EMPLOYEE
go
if exists (select 1
            from  sysobjects
           where  id = object_id('GENRE')
            and   type = 'U')
   drop table GENRE
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('HASACTOR')
            and   name  = 'HASACTOR_FK'
            and   indid > 0
            and   indid < 255)
   drop index HASACTOR.HASACTOR_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('HASACTOR')
            and   name  = 'HASACTOR2_FK'
            and   indid > 0
            and   indid < 255)
   drop index HASACTOR.HASACTOR2_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('HASACTOR')
            and   type = 'U')
   drop table HASACTOR
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('HASGENRE')
            and   name  = 'HASGENRE_FK'
            and   indid > 0
            and   indid < 255)
   drop index HASGENRE.HASGENRE_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('HASGENRE')
            and   name  = 'HASGENRE2_FK'
            and   indid > 0
            and   indid < 255)
   drop index HASGENRE.HASGENRE2_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('HASGENRE')
            and   type = 'U')
   drop table HASGENRE
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('MOVIE')
            and   name  = 'HASAGERATING_FK'
            and   indid > 0
            and   indid < 255)
   drop index MOVIE.HASAGERATING_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('MOVIE')
            and   name  = 'PRODUCEDINNATION_FK'
            and   indid > 0
            and   indid < 255)
   drop index MOVIE.PRODUCEDINNATION_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('MOVIE')
            and   name  = 'DIRECTEDBY_FK'
            and   indid > 0
            and   indid < 255)
   drop index MOVIE.DIRECTEDBY_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('MOVIE')
            and   type = 'U')
   drop table MOVIE
go
if exists (select 1
            from  sysobjects
           where  id = object_id('NATION')
            and   type = 'U')
   drop table NATION
go
if exists (select 1
            from  sysobjects
           where  id = object_id('PARAMETER')
            and   type = 'U')
   drop table PARAMETER
go
if exists (select 1
            from  sysobjects
           where  id = object_id('PAYMENT')
            and   type = 'U')
   drop table PAYMENT
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('REVIEW')
            and   name  = 'WRITES_FK'
            and   indid > 0
            and   indid < 255)
   drop index REVIEW.WRITES_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('REVIEW')
            and   name  = 'HASREVIEWS_FK'
            and   indid > 0
            and   indid < 255)
   drop index REVIEW.HASREVIEWS_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('REVIEW')
            and   type = 'U')
   drop table REVIEW
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('SEAT')
            and   name  = 'CONTAINSSEATS_FK'
            and   indid > 0
            and   indid < 255)
   drop index SEAT.CONTAINSSEATS_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('SEAT')
            and   type = 'U')
   drop table SEAT
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('SHOWTIME')
            and   name  = 'ISSHOWNIN_FK'
            and   indid > 0
            and   indid < 255)
   drop index SHOWTIME.ISSHOWNIN_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('SHOWTIME')
            and   name  = 'HOSTS_FK'
            and   indid > 0
            and   indid < 255)
   drop index SHOWTIME.HOSTS_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('SHOWTIME')
            and   type = 'U')
   drop table SHOWTIME
go
if exists (select 1
            from  sysobjects
           where  id = object_id('THEATER')
            and   type = 'U')
   drop table THEATER
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKET')
            and   name  = 'SELLS_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKET.SELLS_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKET')
            and   name  = 'HASTICKETS_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKET.HASTICKETS_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKET')
            and   name  = 'PURCHASES_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKET.PURCHASES_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKET')
            and   name  = 'ISPAIDBY_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKET.ISPAIDBY_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('TICKET')
            and   type = 'U')
   drop table TICKET
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKETDETAIL')
            and   name  = 'HASPRICE_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKETDETAIL.HASPRICE_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKETDETAIL')
            and   name  = 'INCLUDESDETAIL_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKETDETAIL.INCLUDESDETAIL_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('TICKETDETAIL')
            and   type = 'U')
   drop table TICKETDETAIL
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKETPRICE')
            and   name  = 'APPLIESTODAYCAT_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKETPRICE.APPLIESTODAYCAT_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKETPRICE')
            and   name  = 'APPLIESTOTYPE_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKETPRICE.APPLIESTOTYPE_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('TICKETPRICE')
            and   type = 'U')
   drop table TICKETPRICE
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKETSEAT')
            and   name  = 'HASSEAT_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKETSEAT.HASSEAT_FK
go
if exists (select 1
            from  sysindexes
           where  id    = object_id('TICKETSEAT')
            and   name  = 'ASSIGNS_FK'
            and   indid > 0
            and   indid < 255)
   drop index TICKETSEAT.ASSIGNS_FK
go
if exists (select 1
            from  sysobjects
           where  id = object_id('TICKETSEAT')
            and   type = 'U')
   drop table TICKETSEAT
go
if exists (select 1
            from  sysobjects
           where  id = object_id('TICKETTYPE')
            and   type = 'U')
   drop table TICKETTYPE
go
create table ACTOR (
   ACTORID              bigint               identity,
   ACTORNAME            nvarchar(50)          not null,
   constraint PK_ACTOR primary key (ACTORID)
)
go
create table AGERATING (
   AGEID                int                  identity,
   AGERATING            nvarchar(5)           not null,
   constraint PK_AGERATING primary key (AGEID)
)
go
create table CUSTOMER (
   CUSTOMERID           bigint               identity,
   LASTNAME             nvarchar(20)          not null,
   FIRSTNAME            nvarchar(20)          not null,
   CEMAIL               varchar(50)          not null,
   CPHONE               varchar(10)          not null,
   CPASSWORD            varchar(50)          not null,
   CDOB                 datetime             not null,
   constraint PK_CUSTOMER primary key (CUSTOMERID)
)
go
create table DAYCAT (
   DAYCATID             int                  identity,
   DAYCATNAME           nvarchar(20)          not null,
   constraint PK_DAYCAT primary key (DAYCATID)
)
go
create table DIRECTOR (
   DIRECTORID           bigint               identity,
   DIRECTORNAME         nvarchar(50)          not null,
   constraint PK_DIRECTOR primary key (DIRECTORID)
)
go
create table EMPLOYEE (
   EMPLOYEEID           bigint               identity,
   ELASTNAME            nvarchar(20)          not null,
   EFIRSTNAME           nvarchar(20)          not null,
   EEMAIL               varchar(50)          not null,
   EPHONE               varchar(10)          not null,
   EPASSWORD            varchar(50)          not null,
   EDOB                 datetime             not null,
   EADDRESS             nvarchar(max)                 not null,
   EDATEOFWORK          datetime             not null,
   ROLENAME             varchar(20)          not null,
   constraint PK_EMPLOYEE primary key (EMPLOYEEID)
)
go
create table GENRE (
   GENREID              bigint               identity,
   GENRENAME            nvarchar(50)          not null,
   constraint PK_GENRE primary key (GENREID)
)
go
create table HASACTOR (
   MOVIEID              bigint               not null,
   ACTORID              bigint               not null,
   constraint PK_HASACTOR primary key (MOVIEID, ACTORID)
)
go
create nonclustered index HASACTOR2_FK on HASACTOR (MOVIEID ASC)
go
create nonclustered index HASACTOR_FK on HASACTOR (ACTORID ASC)
go
create table HASGENRE (
   GENREID              bigint               not null,
   MOVIEID              bigint               not null,
   constraint PK_HASGENRE primary key (GENREID, MOVIEID)
)
go
create nonclustered index HASGENRE2_FK on HASGENRE (GENREID ASC)
go
create nonclustered index HASGENRE_FK on HASGENRE (MOVIEID ASC)
go
create table MOVIE (
   MOVIEID              bigint               identity,
   DIRECTORID           bigint               not null,
   NATIONID             int                  not null,
   AGEID                int                  not null,
   MOVIENAME            nvarchar(150)         not null,
   DURATION             int                  not null,
   DESCRIPTION          nvarchar(max)               null,
   RELEASEDATE          datetime             not null,
   MOVIESTATUS          nvarchar(50)          not null,
   POSTER               nvarchar(max)          null,
   constraint PK_MOVIE primary key (MOVIEID)
)
go
create nonclustered index DIRECTEDBY_FK on MOVIE (DIRECTORID ASC)
go
create nonclustered index PRODUCEDINNATION_FK on MOVIE (NATIONID ASC)
go
create nonclustered index HASAGERATING_FK on MOVIE (AGEID ASC)
go
create table NATION (
   NATIONID             int                  identity,
   NATIONNAME           nvarchar(30)          not null,
   constraint PK_NATION primary key (NATIONID)
)
go
create table PARAMETER (
   PARAMETERID          bigint               identity,
   PARAMETERDESCRIPTION nvarchar(max)                 not null,
   VALUE                nvarchar(20)          not null,
   UNITOFMEASUREMENT    nvarchar(20)          not null,
   APPLYCATION          bit                  not null,
   constraint PK_PARAMETER primary key (PARAMETERID)
)
go
create table PAYMENT (
   PAYMENTID            bigint               identity,
   PAYMENTMETHOD        nvarchar(100)         not null,
   constraint PK_PAYMENT primary key (PAYMENTID)
)
go
create table REVIEW (
   REVIEWID             bigint               identity,
   MOVIEID              bigint               not null,
   CUSTOMERID           bigint               not null,
   COMMENT              nvarchar(max)                 not null,
   VOTE                 int                  not null,
   REVIEWDATE           datetime             not null,
   constraint PK_REVIEW primary key (REVIEWID)
)
go
create nonclustered index HASREVIEWS_FK on REVIEW (MOVIEID ASC)
go
create nonclustered index WRITES_FK on REVIEW (CUSTOMERID ASC)
go
create table SEAT (
   SEATID               bigint               identity,
   THEATERID            bigint               not null,
   SEATNAME             varchar(5)           not null,
   constraint PK_SEAT primary key (SEATID)
)
go
create nonclustered index CONTAINSSEATS_FK on SEAT (THEATERID ASC)
go
create table SHOWTIME (
   SHOWTIMEID           bigint               identity,
   THEATERID            bigint               not null,
   MOVIEID              bigint               not null,
   STARTTIME            datetime             not null,
   ENDTIME              datetime             not null,
   constraint PK_SHOWTIME primary key (SHOWTIMEID)
)
go
create nonclustered index HOSTS_FK on SHOWTIME (THEATERID ASC)
go
create nonclustered index ISSHOWNIN_FK on SHOWTIME (MOVIEID ASC)
go
create table THEATER (
   THEATERID            bigint               identity,
   THEATERNAME          nvarchar(10)          not null,
   constraint PK_THEATER primary key (THEATERID)
)
go
create table TICKET (
   TICKETID             int                  identity,
   PAYMENTID            bigint               not null,
   CUSTOMERID           bigint               null,
   SHOWTIMEID           bigint               not null,
   EMPLOYEEID           bigint               not null,
   TOTALTICKETS         int                  not null,
   TOTALAMOUT           decimal(12,2)        not null,
   TICKETSTATUS         bit                  not null,
   SELLDATE             datetime             not null,
   constraint PK_TICKET primary key (TICKETID)
)
go
create nonclustered index ISPAIDBY_FK on TICKET (PAYMENTID ASC)
go
create nonclustered index PURCHASES_FK on TICKET (CUSTOMERID ASC)
go
create nonclustered index HASTICKETS_FK on TICKET (SHOWTIMEID ASC)
go
create nonclustered index SELLS_FK on TICKET (EMPLOYEEID ASC)
go
create table TICKETDETAIL (
   TICKETDETAILID       bigint               identity,
   TICKETID             int                  not null,
   TICKETPRICEID        int                  not null,
   QUANTITY             int                  not null,
   TOTALPRICE           decimal(12,2)        not null,
   constraint PK_TICKETDETAIL primary key (TICKETDETAILID)
)
go
create nonclustered index INCLUDESDETAIL_FK on TICKETDETAIL (TICKETID ASC)
go
create nonclustered index HASPRICE_FK on TICKETDETAIL (TICKETPRICEID ASC)
go
create table TICKETPRICE (
   TICKETPRICEID        int                  identity,
   TICKETTYPEID         int                  not null,
   DAYCATID             int                  not null,
   PRICETTICKET         decimal(12,2)        not null,
   constraint PK_TICKETPRICE primary key (TICKETPRICEID)
)
go
create nonclustered index APPLIESTOTYPE_FK on TICKETPRICE (TICKETTYPEID ASC)
go
create nonclustered index APPLIESTODAYCAT_FK on TICKETPRICE (DAYCATID ASC)
go
create table TICKETSEAT (
   TICKETSEATID         bigint               identity,
   TICKETDETAILID       bigint               not null,
   SEATID               bigint               not null,
   constraint PK_TICKETSEAT primary key (TICKETSEATID)
)
go
create nonclustered index ASSIGNS_FK on TICKETSEAT (TICKETDETAILID ASC)
go
create nonclustered index HASSEAT_FK on TICKETSEAT (SEATID ASC)
go
create table TICKETTYPE (
   TICKETTYPEID         int                  identity,
   TICKETTYPENAME       nvarchar(30)          not null,
   constraint PK_TICKETTYPE primary key (TICKETTYPEID)
)
go
alter table HASACTOR
   add constraint FK_HASACTOR_HASACTOR_ACTOR foreign key (ACTORID)
      references ACTOR (ACTORID)
go
alter table HASACTOR
   add constraint FK_HASACTOR_HASACTOR2_MOVIE foreign key (MOVIEID)
      references MOVIE (MOVIEID)
go
alter table HASGENRE
   add constraint FK_HASGENRE_HASGENRE_MOVIE foreign key (MOVIEID)
      references MOVIE (MOVIEID)
go
alter table HASGENRE
   add constraint FK_HASGENRE_HASGENRE2_GENRE foreign key (GENREID)
      references GENRE (GENREID)
go
alter table MOVIE
   add constraint FK_MOVIE_DIRECTEDB_DIRECTOR foreign key (DIRECTORID)
      references DIRECTOR (DIRECTORID)
go
alter table MOVIE
   add constraint FK_MOVIE_HASAGERAT_AGERATIN foreign key (AGEID)
      references AGERATING (AGEID)
go
alter table MOVIE
   add constraint FK_MOVIE_PRODUCEDI_NATION foreign key (NATIONID)
      references NATION (NATIONID)
go
alter table REVIEW
   add constraint FK_REVIEW_HASREVIEW_MOVIE foreign key (MOVIEID)
      references MOVIE (MOVIEID)
go
alter table REVIEW
   add constraint FK_REVIEW_WRITES_CUSTOMER foreign key (CUSTOMERID)
      references CUSTOMER (CUSTOMERID)
go
alter table SEAT
   add constraint FK_SEAT_CONTAINSS_THEATER foreign key (THEATERID)
      references THEATER (THEATERID)
go
alter table SHOWTIME
   add constraint FK_SHOWTIME_HOSTS_THEATER foreign key (THEATERID)
      references THEATER (THEATERID)
go
alter table SHOWTIME
   add constraint FK_SHOWTIME_ISSHOWNIN_MOVIE foreign key (MOVIEID)
      references MOVIE (MOVIEID)
go
alter table TICKET
   add constraint FK_TICKET_HASTICKET_SHOWTIME foreign key (SHOWTIMEID)
      references SHOWTIME (SHOWTIMEID)
go
alter table TICKET
   add constraint FK_TICKET_ISPAIDBY_PAYMENT foreign key (PAYMENTID)
      references PAYMENT (PAYMENTID)
go
alter table TICKET
   add constraint FK_TICKET_PURCHASES_CUSTOMER foreign key (CUSTOMERID)
      references CUSTOMER (CUSTOMERID)
go
alter table TICKET
   add constraint FK_TICKET_SELLS_EMPLOYEE foreign key (EMPLOYEEID)
      references EMPLOYEE (EMPLOYEEID)
go
alter table TICKETDETAIL
   add constraint FK_TICKETDE_HASPRICE_TICKETPR foreign key (TICKETPRICEID)
      references TICKETPRICE (TICKETPRICEID)
go
alter table TICKETDETAIL
   add constraint FK_TICKETDE_INCLUDESD_TICKET foreign key (TICKETID)
      references TICKET (TICKETID)
go
alter table TICKETPRICE
   add constraint FK_TICKETPR_APPLIESTO_DAYCAT foreign key (DAYCATID)
      references DAYCAT (DAYCATID)
go
alter table TICKETPRICE
   add constraint FK_TICKETPR_APPLIESTO_TICKETTY foreign key (TICKETTYPEID)
      references TICKETTYPE (TICKETTYPEID)
go
alter table TICKETSEAT
   add constraint FK_TICKETSE_ASSIGNS_TICKETDE foreign key (TICKETDETAILID)
      references TICKETDETAIL (TICKETDETAILID)
go
alter table TICKETSEAT
   add constraint FK_TICKETSE_HASSEAT_SEAT foreign key (SEATID)
      references SEAT (SEATID)
go

--Insert data
INSERT INTO CUSTOMER (LASTNAME, FIRSTNAME, CEMAIL, CPHONE, CPASSWORD, CDOB)
VALUES 
--matkhau123
(N'Nguyễn', N'An', N'nguyenan@gmail.com', '0912345678', '1a24b2c4b17016486c67773b974181bc', '1995-04-20'),
--abcXYZ2024
(N'Trần', N'Bình', N'tranbinh@gmail.com', '0987654321', '8f437b9a060fca42cf6753d2218e1d93', '1990-12-10'),
--lecuong321
(N'Lê', N'Cường', N'lecuong@gmail.com', '0909123456', 'c498c3bdd887da4c8628c589e0d2bdb0', '1988-08-15'),
(N'Nguyễn Thành', N'Lâm', N'nguyenlam@gmail.com', '0912345678', '1a24b2c4b17016486c67773b974181bc', '1995-04-20'),
(N'Trần Ngọc', N'Phương', N'tranphuong@gmail.com', '0987654321', '8f437b9a060fca42cf6753d2218e1d93', '1990-12-10'),
(N'Lê Hoàng', N'Trung', N'trungle@gmail.com', '0909123456', 'c498c3bdd887da4c8628c589e0d2bdb0', '1988-08-15'),
(N'Phạm Thế', N'An', N'phaman@gmail.com', '0912345678', '1a24b2c4b17016486c67773b974181bc', '1995-04-20'),
(N'Trần Nhật', N'Bình', N'nhatbinh@gmail.com', '0987654321', '8f437b9a060fca42cf6753d2218e1d93', '1990-12-10'),
(N'Cao Minh', N'Phong', N'caominhphong@gmail.com', '0909123456', 'c498c3bdd887da4c8628c589e0d2bdb0', '1988-08-15'),
(N'Diệp', N'Minh', N'diepminh@gmail.com', '0912345678', '1a24b2c4b17016486c67773b974181bc', '1995-04-20'),
(N'Lê Hoàng Minh', N'Hải', N'hailhm@gmail.com', '0987654321', '8f437b9a060fca42cf6753d2218e1d93', '1990-12-10'),
(N'Phạm Thị', N'Hoa', N'hoapham@gmail.com', '0909123456', 'c498c3bdd887da4c8628c589e0d2bdb0', '1988-08-15');

INSERT INTO EMPLOYEE (ELASTNAME, EFIRSTNAME, EEMAIL, EPHONE, EPASSWORD, EDOB, EADDRESS, EDATEOFWORK, ROLENAME)
VALUES
--matkhau01
(N'Phạm', N'Duy', 'duypham@gmail.com', '0909123456', 'b0ee3050c15d96f5d6563a13d6d68220', '1990-01-15', N'123 Lê Lợi, Nha Trang', '2015-06-01', 'Admin'),
(N'Hàng Bảo', N'Ngọc', 'hangbngoc@gmail.com', '0909123456', 'b0ee3050c15d96f5d6563a13d6d68220', '1990-01-15', N'123 Lê Lợi, Nha Trang', '2015-06-01', 'Admin'),
---abc123
(N'Lê Bảo', N'Chiến', 'baochien@gmail.com', '0911222333', 'e99a18c428cb38d5f260853678922e03', '1988-04-20', N'456 Nguyễn Trãi, Hà Nội', '2017-09-15', 'Employee'),
(N'Hồ Hoàng', N'Diệp', 'diephoang@gmail.com', '0933444555', 'e99a18c428cb38d5f260853678922e03', '1992-07-10', N'789 Lý Thường Kiệt, Nha Trang', '2020-01-20', 'Employee'),
(N'Nguyễn Chí', N'Trường', 'chitruong@gmail.com', '0911222333', 'e99a18c428cb38d5f260853678922e03', '1988-04-20', N'456 Nguyễn Trãi, Hà Nội', '2017-09-15', 'Employee'),
(N'Lê Thiện', N'Hoàng', 'hoangthien@gmail.com', '0933444555', 'e99a18c428cb38d5f260853678922e03', '1992-07-10', N'789 Lý Thường Kiệt, Nha Trang', '2020-01-20', 'Employee');

INSERT INTO NATION (NATIONNAME) VALUES
(N'Argentina'), 
(N'Ấn Độ'),
(N'Bỉ'),
(N'Bồ Đào Nha'),
(N'Brazil'),
(N'Đài Loan'),
(N'Đan Mạch'),
(N'Đức'),
(N'Anh'),
(N'Hàn Quốc'),
(N'Indonesia'),
(N'Mexico'),
(N'Mỹ'),
(N'Nhật Bản'),
(N'Pháp'),
(N'Thái Lan'),
(N'Thổ Nhĩ Kỳ'),
(N'Việt Nam');

INSERT INTO AGERATING (AGERATING) VALUES
('P'),
(N'K'),
(N'T13'),
(N'T16'),
(N'T18'),
(N'C');
go

INSERT INTO GENRE (GENRENAME) VALUES
(N'Cao bồi'),
(N'Chiến tranh'),
(N'Gia đình'),
(N'Giả tưởng'),
(N'Giật gân'),
(N'Hài'),
(N'Hành động'),
(N'Hình sự'),
(N'Hoạt hình'),
(N'Kinh dị'),
(N'Lãng mạn'),
(N'Lịch sử'),
(N'Ly kì'),
(N'Nhạc kịch'),
(N'Phiêu lưu'),
(N'Tài liệu'),
(N'Tâm lý'),
(N'Thần thoại'),
(N'Thể thao'),
(N'Tiểu sử'),
(N'Tình cảm'),
(N'Tội phạm');
go

INSERT INTO ACTOR (ACTORNAME) VALUES
(N'Quốc Huy'),
(N'Đinh Ngọc Diệp'),
(N'Trần Quốc Anh'),
(N'Florence Pugh'),
(N'Sebastian Stan'),
(N'Rachel Weisz'),
(N'Hossein Saffarzadegan'),
(N'David Harbour'),
(N'Wyatt Russell'),
(N'NSƯT Kim Phương'),
(N'Long Đẹp Trai'),
(N'NSƯT Tuyết Thu'),
(N'Quách Ngọc Tuyên'),
(N'Đoàn Thế Vinh'),
(N'Thái Hòa'),
(N'Hồ Thu Anh'),
(N'Quang Tuấn'),
(N'NSƯT Cao Minh'),
(N'Figen Aksoy'),
(N'Fuda Askin'),
(N'Eguchi Takuya'),
(N'Ben Affleck'),
(N'Jon Bernthal'),
(N'J.K Simmons'),
(N'Kobayashi Yumiko'),
(N'Narahashi Miki'),
(N'Morikawa Toshiyuki');
go

INSERT INTO DIRECTOR (DIRECTORNAME) VALUES
(N'Victor Vũ'),
(N'Jake Schreier'),
(N'Lý Hải'),
(N'Bùi Thạc Chuyên'),
(N'Meisam Hosseini'),
(N'Gavin O Connor'),
(N'Takahashi Wataru');
go

INSERT INTO MOVIE (DIRECTORID, NATIONID, AGEID, MOVIENAME, DURATION, DESCRIPTION, RELEASEDATE, MOVIESTATUS, POSTER)
VALUES
(1, 18, 4, N'Thám Tử Kiên: Kỳ Án Không Đầu', 182, N'Thám Tử Kiên là một nhân vật được yêu thích trong tác phẩm điện của ăn khách Người Vợ Cuối Cùng của Victor Vũ, Thám Tử Kiên: Kỳ Không Đầu sẽ là một phim Victor Vũ trở về với thể loại sở trường Kinh Dị - Trinh Thám sau những tác phẩm tình cảm lãng mạn trước đó.', '2025-04-25', 'Released', 'https://cdn.galaxycine.vn/media/2025/4/28/tham-tu-kien-2_1745832748529.jpg'),
(2, 13, 3, N'Biệt Đội Sấm Sét*', 126, N'Thunderbolts* là bộ phim quan trọng trong quá trình khép lại Kỷ nguyên anh hùng thứ năm (Phase Five) và chuẩn bị mở đường cho siêu bom tấn Avengers: Doomsday trong năm sau. Đây là tác phẩm được fan cực kỳ mong mỏi khi quy tụ dàn nhân vật quen thuộc để tạo thành nhóm siêu anh hùng mới. Họ đều là các siêu chiến binh, sát thủ với kỹ năng chiến đấu thượng thừa. Chờ xem những phản anh hùng ngày trước sẽ thay đổi thế nào ở Thunderbolts*/ Biệt Đội Sấm Sét*.', '2025-04-30', 'Released', 'https://cdn.galaxycine.vn/media/2025/4/23/thunderbolts-500_1745395912649.jpg'),
(3, 18, 3, N'Lật Mặt 8: Vòng Tay Nắng', 135, N'Lật Mặt 8: Vòng Tay Nắng kể về sự khác biệt quan điểm giữa ba thế hệ ông bà cha mẹ con cháu. Ai cũng đúng ở góc nhìn của mình nhưng đứng trước hoài bão của tuổi trẻ, cuối cùng thì ai sẽ là người phải nghe theo người còn lại? Và nếu ước mơ của những đứa trẻ bị cho là viển vông, thì cơ hội nào và bao giờ tuổi trẻ mới được tự quyết định tương lai của mình?', '2025-04-27', 'Released', 'https://cdn.galaxycine.vn/media/2025/4/21/lm8-500_1745206373828.jpg'),
(4, 18, 4, N'Địa Đạo: Mặt Trời Trong Bóng Tối', 125, N'Địa Đạo: Mặt Trời Trong Bóng Tối là dự án điện ảnh kỷ niệm 50 năm hòa bình thống nhất đất nước, dự kiến khởi chiếu 30.04.2025. Phim do đạo diễn Bùi Thạc Chuyên cầm trịch, với sự tham gia của dàn diễn viên thực lực – Thái Hòa, Quang Tuấn và diễn viên trẻ Hồ Thu Anh. Vào năm 1967, chiến tranh Việt Nam ngày càng khốc liệt. Đội du kích 21 người do BẢY THEO chỉ huy tại căn cứ Bình An Đông trở thành mục tiêu mà quân đội Mỹ TÌM VÀ DIỆT số 1 khi nhận nhiệm vụ bằng mọi giá phải bảo vệ một nhóm thông tin tình báo chiến lược mới đến ẩn náu tại căn cứ. Các cuộc liên lạc vô tuyến điện từ với nhóm tình báo bị quân đội Mỹ phát hiện và định vị, lấy đi lợi thế duy nhất của đội du kích là sự vô hình trong hệ thống địa đạo rộng khắp, phức tạp và bí ẩn.', '2025-04-02', 'Released', 'https://cdn.galaxycine.vn/media/2025/4/29/dia-dao-500_1745896687306.jpg'),
(5, 17, 1, N'Chuyện Muông Thú Dạy Bé Cừu Bay', 81, N'Sống trong thế giới nơi chỉ có loài chim mới được phép bay đã giới hạn ước mơ của biết bao giống loài, điển hình là bé cừu nhỏ Woolina.Trên hành trình chinh phục giấc mơ không tưởng, Woolina đã sát cánh cùng hội bạn Beep Beep, Momo,..dù nhận lấy sự phản đối từ mọi người. Liệu với trái tim nhỏ bé đầy dũng cảm ấy, Woolina có phá vỡ được định kiến và hiện thực hóa được giấc mơ bay lượn tự do trên bầu trời nhỏ của mình?', '2025-04-19', 'Released', 'https://cdn.galaxycine.vn/media/2025/4/15/woolina-500_1744702775247.jpg'),
(6, 13, 5, N'Mật Danh: Kế Toán 2', 133, N'The Accountant 2 / Mật Danh: Kế Toán 2 kể vềChristian Wolff (Ben Affleck) sở hữu tài năng giải quyết những vấn đề phức tạp. Khi một người bạn cũ bị ám sát, để lại một thông điệp mơ hồ yêu cầu "truy tìm kế toán", Wolff bị cuốn vào việc giải quyết vụ án. Nhận ra rằng cần phải có những biện pháp cực đoan, Wolff đã liên lạc với người anh trai xa cách và vô cùng nguy hiểm của mình, Brax (Jon Bernthal), để cùng nhau điều tra. Cùng với Phó Giám đốc Cục Tài chính Marybeth Medina (Cynthia Addai-Robinson), họ phát hiện ra một âm mưu chết người, trở thành mục tiêu của một mạng lưới sát thủ tàn bạo, sẵn sàng làm mọi thứ để chôn vùi những bí mật của mình.', '2025-05-02', 'Released', 'https://cdn.galaxycine.vn/media/2025/2/24/mat-danh-ke-toan-2-500_1740369432802.jpg'),
(7, 14, 1, N'Phim Shin Cậu Bé Bút Chì: Bí Ẩn! Học Viện Hoa Lệ Tenkasu', 106, N'Phim Shin Cậu Bé Bút Chì: Bí Ẩn! Học Viện Hoa Lệ Tenkasu kể về trải nghiệm học tại Học viện Tenkasu của bé Shin. Shinnosuke và những người bạn của Shin thuộc Đội đặc nhiệm Kasukabe trải qua một tuần ở lại "Học viện Tư nhân Tenkasu Kasukabe" (Học viện Tenkasu). Đây là trường nội trú ưu tú được quản lý bởi một AI hiện đại - Otsmun.  Tất cả các học sinh ban đầu được trao một huy hiệu với 1000 điểm và điểm của các em sẽ được Otsmun tăng hoặc giảm dựa trên hành vi và kết quả học tập của các em. Tuy nhiên, ai đó đã tấn công Kazama. Đội đặc nhiệm Kasukabe hợp lực với chủ tịch hội học sinh bỏ học của trường, Chishio Atsuki, một cựu vận động viên, để thành lập một nhóm thám tử và giải quyết bí ẩn.', '2025-04-30', 'Released', 'https://cdn.galaxycine.vn/media/2025/4/10/crayon-shinchan-the-movie-school-mystery-the-splendid-tenkasu-academy-2_1744271074944.jpg');
go

INSERT INTO HASACTOR (MOVIEID, ACTORID) VALUES
(1, 1), (1, 2), (1, 3),
(2, 4), (2, 5), (2, 6), (2, 8), (2, 9),
(3, 10), (3, 11), (3, 12), (3, 13), (3, 14),
(4, 15), (4, 16), (4, 17), (4, 18),
(5, 19), (5, 20), (5, 21), (5, 7),
(6, 22), (6, 23), (6, 24),
(7, 25), (7, 26), (7, 27);
go

INSERT INTO HASGENRE (GENREID, MOVIEID) VALUES
(10, 1), (17, 1),
(7, 2), (4, 2), (15, 2),
(3, 3), (21, 3), (17, 3),
(7, 4), (12, 4),
(9, 5),
(7, 6), (22, 6),
(9, 7);
go

INSERT INTO DAYCAT (DAYCATNAME)
VALUES
(N'Ngày trong tuần'),
(N'Ngày cuối tuần'),
(N'Ngày lễ'),
(N'Happy day');

INSERT INTO TICKETTYPE (TICKETTYPENAME)
VALUES
(N'U22'),
(N'Người cao tuổi/Trẻ em'),
(N'Người lớn');

-- Happy day
INSERT INTO TICKETPRICE (TICKETTYPEID, DAYCATID, PRICETTICKET)
VALUES
(1, 4, 55000), -- U22
(2, 4, 55000), -- Người cao tuổi/Trẻ em
(3, 4, 55000); -- Người lớn

-- Ngày trong tuần
INSERT INTO TICKETPRICE (TICKETTYPEID, DAYCATID, PRICETTICKET)
VALUES
(1, 1, 55000),
(2, 1, 55000),
(3, 1, 60000);

-- Ngày cuối tuần
INSERT INTO TICKETPRICE (TICKETTYPEID, DAYCATID, PRICETTICKET)
VALUES
(1, 2, 55000),
(2, 2, 55000),
(3, 2, 70000);

-- Ngày lễ
INSERT INTO TICKETPRICE (TICKETTYPEID, DAYCATID, PRICETTICKET)
VALUES
(1, 3, 70000),
(2, 3, 60000),
(3, 3, 80000);

INSERT INTO PAYMENT (PAYMENTMETHOD)
VALUES
(N'Tiền mặt'),      -- Cash
(N'Chuyển khoản'); 

INSERT INTO Theater (theaterName)
VALUES
    (N'Phòng 1'),
    (N'Phòng 2'),
    (N'Phòng 3'),
    (N'Phòng 4'),
	(N'Phòng 5');
GO

insert into SHOWTIME (THEATERID, MOVIEID, STARTTIME, ENDTIME)
values 
(3, 7, '2025-05-03 09:00:00', '2025-05-03 10:44:00'),

(2, 4, '2025-05-03 09:15:00', '2025-05-03 11:20:00'),

(3, 2, '2025-05-03 11:00:00', '2025-05-03 13:03:00'),

(4, 3, '2025-05-03 09:45:00', '2025-05-03 12:00:00'),
(2, 3, '2025-05-03 11:30:00', '2025-05-03 13:45:00'),
(4, 3, '2025-05-03 12:15:00', '2025-05-03 14:30:00'),
(2, 3, '2025-05-03 14:00:00', '2025-05-03 16:15:00'),
(4, 3, '2025-05-03 14:45:00', '2025-05-03 17:00:00'),
(2, 3, '2025-05-03 16:30:00', '2025-05-03 18:45:00'),
(4, 3, '2025-05-03 17:15:00', '2025-05-03 19:30:00'),
(2, 3, '2025-05-03 19:00:00', '2025-05-03 21:15:00'),
(4, 3, '2025-05-03 19:45:00', '2025-05-03 22:00:00'),
(4, 3, '2025-05-03 22:15:00', '2025-05-03 00:35:00'),

(5, 1, '2025-05-03 09:30:00', '2025-05-03 11:41:00'),
(1, 1, '2025-05-03 10:30:00', '2025-05-03 12:41:00'),
(5, 1, '2025-05-03 12:00:00', '2025-05-03 14:11:00'),
(1, 1, '2025-05-03 13:00:00', '2025-05-03 15:11:00'),
(5, 1, '2025-05-03 14:30:00', '2025-05-03 16:41:00'),
(1, 1, '2025-05-03 15:30:00', '2025-05-03 17:41:00'),
(5, 1, '2025-05-03 17:00:00', '2025-05-03 19:11:00'),
(1, 1, '2025-05-03 18:00:00', '2025-05-03 20:11:00'),
(5, 1, '2025-05-03 19:30:00', '2025-05-03 21:41:00'),
(1, 1, '2025-05-03 20:20:00', '2025-05-03 22:31:00'),
(5, 1, '2025-05-03 22:00:00', '2025-05-03 00:11:00');

INSERT INTO REVIEW (MOVIEID, CUSTOMERID, COMMENT, VOTE, REVIEWDATE) VALUES
(1, 1, N'Phim rất hấp dẫn, kỹ xảo đẹp.', 5, '2025-04-25'),
(2, 2, N'Tình tiết hơi chậm, nhưng diễn xuất tốt.', 3, '2025-04-30'),
(3, 1, N'Tuyệt vời! Một kiệt tác điện ảnh.', 5, '2025-04-27'),
(1, 4, N'Phim khá ổn, kết thúc hơi dễ đoán.', 4, '2025-04-26'),
(4, 5, N'Thất vọng, không như kỳ vọng.', 2, '2025-04-05'),
(2, 6, N'Không tệ, phù hợp để giải trí cuối tuần.', 3, '2025-04-30'),
(3, 7, N'Một trong những phim hay nhất năm.', 5, '2025-04-27'),
(7, 6, N'Chưa thực sự nổi bật.', 3, '2025-05-01'),
(1, 9, N'Kịch bản rất sáng tạo, thích cách dẫn dắt.', 4, '2025-04-26'),
(4, 10, N'Nhịp phim không đồng đều, dễ chán.', 2, '2025-04-10'),
(3, 11, N'Cốt truyện sâu sắc, nhạc phim tuyệt vời.', 5, '2025-04-29'),
(2, 12, N'Phim ổn nhưng diễn viên chính chưa nổi bật.', 3, '2025-05-01'),
(7, 3, N'Trung bình, không để lại nhiều ấn tượng.', 3, '2025-05-02'),
(1, 7, N'Phim đáng xem, hình ảnh chất lượng cao.', 4, '2025-04-27'),
(4, 10, N'Thời lượng dài quá, dễ gây mệt.', 2, '2025-04-15');

;WITH 
    -- Danh sách hàng A–J
    Rows AS (
        SELECT 'A' AS RowChar UNION ALL
        SELECT 'B'        UNION ALL
        SELECT 'C'        UNION ALL
        SELECT 'D'        UNION ALL
        SELECT 'E'        UNION ALL
        SELECT 'F'        UNION ALL
        SELECT 'G'        UNION ALL
        SELECT 'H'        UNION ALL
        SELECT 'I'        UNION ALL
        SELECT 'J'
    ),
    -- Danh sách số 1–10
    Numbers AS (
        SELECT 1 AS Num UNION ALL
        SELECT 2       UNION ALL
        SELECT 3       UNION ALL
        SELECT 4       UNION ALL
        SELECT 5       UNION ALL
        SELECT 6       UNION ALL
        SELECT 7       UNION ALL
        SELECT 8       UNION ALL
        SELECT 9       UNION ALL
        SELECT 10
    )
INSERT INTO Seat (theaterID, seatName)
SELECT
    t.theaterID,
    Rows.RowChar + CAST(Numbers.Num AS VARCHAR(2)) AS seatName
FROM Theater t
CROSS JOIN Rows
CROSS JOIN Numbers
WHERE t.theaterID BETWEEN 1 AND 5;


