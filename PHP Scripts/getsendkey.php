<?php
include("connect.php");

session_start();

$User = $_POST["user"];

if(!isset($_SESSION['LoggedIn'])) { 
	echo "Login first.";
	exit; 
} 	

$stmt = $conn->prepare("SELECT publickey FROM Users WHERE username = ?");
$stmt->bind_param("s", $User);
$stmt->execute();

$stmt->bind_result($sendkey);
$row = $stmt->fetch();

echo $sendkey;
?>