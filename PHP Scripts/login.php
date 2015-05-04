<?php
session_start();

include("connect.php");
include('Crypt/RSA.php');

$username = $_POST["username"];
$challenge = $_POST["challenge"];

if($challenge == $_SESSION["challenge"] && isset($_SESSION["challenge"]) && ($_SESSION["username"] == $username)) { 

	$rsaserv = new Crypt_RSA();
	extract($rsaserv->createKey());

	$_SESSION["ServerPriv"] = $privatekey;
    $_SESSION["LoggedIn"] = true; 
	
	$_SESSION["ReceiveCounter"] = 0; 
	$_SESSION["SendCounter"] = 0; 
	
	$rsaserv->setPublicKey($publickey);
	
	$stmt = $conn->prepare("SELECT publickey FROM Users WHERE username = ? LIMIT 1");
	$stmt->bind_param("s", $username);
	$stmt->execute();

	$stmt->bind_result($pubkey);
	$row = $stmt->fetch();


	$rsa = new Crypt_RSA();
	$rsa->loadKey($pubkey, CRYPT_RSA_PUBLIC_FORMAT_XML);

	$i = 0;
	$serverpublickey = $rsaserv->getPublicKey(CRYPT_RSA_PUBLIC_FORMAT_XML);
	while ($i < strlen($serverpublickey)) { // right way to do it, safe?
		if ($i + 60 < strlen($serverpublickey)) {
			 echo base64_encode($rsa->encrypt(substr($serverpublickey, $i, 60)))."<br />";
		}
		else {
			echo base64_encode($rsa->encrypt(substr($serverpublickey, $i, strlen($serverpublickey) - $i)))."<br />";
		}
		$i = $i + 60;
	}
} 
else { 
    echo "LoginBad"; 
} 
?>