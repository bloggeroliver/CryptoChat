<?php

include("connect.php");
include('Crypt/RSA.php');
session_start();

$username = $_POST["username"];

$stmt = $conn->prepare("SELECT loginkey FROM Users WHERE username = ? LIMIT 1");
$stmt->bind_param("s", $username);
$stmt->execute();

$stmt->bind_result($pubkey);
$row = $stmt->fetch();


$rsa = new Crypt_RSA();
$rsa->loadKey($pubkey, CRYPT_RSA_PUBLIC_FORMAT_XML);
$challenge = $username.date("Y/m/d").date("h:i:sa").rand(0, 65536);

$_SESSION["username"] = $username; 
$_SESSION["challenge"] = $challenge; 

echo base64_encode($rsa->encrypt($challenge));
?>