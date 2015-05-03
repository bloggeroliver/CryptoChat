<?php
include("connect.php");

$username = $_POST["username"];
$password = $_POST["password"];

$stmt = $conn->prepare("SELECT username FROM Users WHERE username = ?");
$stmt->bind_param("s", $username);
$stmt->execute();

$stmt->store_result();
$num_rows = $stmt->num_rows;

if ($num_rows > 0) {
	echo "Yes";
}
else { 
	echo "No";
}
?>