$testFile = ".\WebApplication.Tests\bin\Release\K9.WebApplication.Tests.dll"
	
function ProcessErrors(){
  if($? -eq $false)
  {
    throw "The previous command failed (see above)";
  }
}

function _Test() {
  echo "Running tests"
  
  packages\xunit.runner.console.2.2.0\tools\xunit.console.exe $testFile
  ProcessErrors
}

function Main {
  Try {
	_Test
  }
  Catch {
    Write-Error $_.Exception
    exit 1
  }
}

Main