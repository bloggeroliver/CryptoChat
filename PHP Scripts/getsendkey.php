<?php
include("connect.php");
include('Crypt/RSA.php');

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

$rand = rand(0, 65536);
$rsa = new Crypt_RSA();
$rsa->loadKey($_SESSION["ServerPriv"]);
$rsa->setSignatureMode(CRYPT_RSA_SIGNATURE_PKCS1);
$signature = $rsa->sign($sendkey."<br />".$rand);

echo base64_encode($signature)."<br />".$sendkey."<br />".$rand;
?>