<?php
include("connect.php");

$username = $_POST["username"];
$loginkey = $_POST["loginkey"];
$sendkey = $_POST["sendkey"];

$stmt = $conn->prepare("SELECT username FROM Users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();

$stmt->store_result();
$num_rows = $stmt->num_rows;

if ($num_rows > 0) {
	echo "Existing";
}
else { 
	$stmt = $conn->prepare("INSERT INTO Users (username, loginkey, sendkey) VALUES (?, ?, ?);");
	$stmt->bind_param("sss", $username, $loginkey, $sendkey);
	$stmt->execute();
	echo "Success";
}
?>