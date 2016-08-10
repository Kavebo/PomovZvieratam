<?php
class ConnectionInfo
{
	public $mServerName;
	public $mConnectionInfo;
	public $conn;
	
	public function GetConnection()
	{
		$this->mServerName = 'myprestage.euweb.cz';
		$this->mConnectionInfo = array("Database"=>"myprestage.e4822", "UUID"=>"myprestage.e4822", "PWD"=>"poilkj067301");
		$this->conn = mysql_connect($this->mServerName, $this->mConnectionInfo);
		
		return $this->conn;
	}
}
?>