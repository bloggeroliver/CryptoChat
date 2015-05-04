<?php
include("connect.php");

$username = $_POST["username"];
$publickey = $_POST["publickey"];
$privatekey = $_POST["privatekey"];

$stmt = $conn->prepare("SELECT username FROM Users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();

$stmt->store_result();
$num_rows = $stmt->num_rows;

if ($num_rows > 0) {
	echo "Existing";
}
else { 
if (isset($privatekey)) {
	$stmt = $conn->prepare("INSERT INTO Users (username, publickey, privatekey) VALUES (?, ?, ?);");
	$stmt->bind_param("sss", $username, $publickey, $privatekey);
	$stmt->execute();
	echo "Success";
}
else {
	$stmt = $conn->prepare("INSERT INTO Users (username, publickey) VALUES (?, ?);");
	$stmt->bind_param("ss", $username, $publickey);
	$stmt->execute();
	echo "Success";
}
}
?>