param([string]$websiteUrl)
function Get-WebPage([string]$url) {
    try{
        $wc = new-object net.webclient
        $pageContents = $wc.DownloadString($url)
        $wc.Dispose()
        return $pageContents
    } catch {}
    return ""
}
$html = Get-WebPage -url $websiteUrl
echo $html