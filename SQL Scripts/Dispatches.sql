USE [master]
GO
/****** Object:  Database [Dispatches]    Script Date: 04/11/2017 15:31:55 ******/
CREATE DATABASE [Dispatches]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Dispatches', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\Dispatches.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Dispatches_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\Dispatches_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Dispatches] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Dispatches].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Dispatches] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Dispatches] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Dispatches] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Dispatches] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Dispatches] SET ARITHABORT OFF 
GO
ALTER DATABASE [Dispatches] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Dispatches] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Dispatches] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Dispatches] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Dispatches] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Dispatches] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Dispatches] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Dispatches] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Dispatches] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Dispatches] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Dispatches] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Dispatches] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Dispatches] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Dispatches] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Dispatches] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Dispatches] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Dispatches] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Dispatches] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Dispatches] SET  MULTI_USER 
GO
ALTER DATABASE [Dispatches] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Dispatches] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Dispatches] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Dispatches] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Dispatches] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Dispatches]
GO
/****** Object:  Table [dbo].[dispatch]    Script Date: 04/11/2017 15:31:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dispatch](
	[id] [int] NOT NULL,
	[orderId] [int] NOT NULL,
 CONSTRAINT [PK_dispatch] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [Dispatches] SET  READ_WRITE 
GO
