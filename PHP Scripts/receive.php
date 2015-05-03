<?php

session_start();

include("connect.php");

if(!isset($_SESSION['username'])) 
   { 
   echo "Bitte erst login";
   exit; 
   } 	
	
$Recipient = $_SESSION['username'];

$stmt = $conn->prepare("SELECT Sender, Message FROM Messages WHERE Recipient = ?");
$stmt->bind_param("s", $Recipient);
$stmt->execute();
	
$stmt->bind_result($sender, $message);
while($row = $stmt->fetch())
   {
		echo "$sender<br />";
		echo "$message<br />";
		$stmt->bind_result($sender, $message);
   }

$stmt = $conn->prepare("DELETE FROM Messages WHERE Recipient = ?");
$stmt->bind_param("s", $Recipient);
$stmt->execute();
?>