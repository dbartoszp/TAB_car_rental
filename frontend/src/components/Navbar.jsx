import NavbarCSS from "./Navbar.module.css";

export default function Navbar() {
  return (
    <header>
      <nav className={NavbarCSS.navigationContainer}>
        <a href="#">
          <img
            src="./src/assets/sogood-rentals-logo-transparent.png"
            className={NavbarCSS.logo}
          ></img>
        </a>
        <ul className={NavbarCSS.navigationList}>
          <li>
            <a href="#">Oferta</a>
          </li>
          <li>
            <a href="#">O nas</a>
          </li>
          <li>
            <a href="#">Dlaczego my?</a>
          </li>
          <li>
            <a href="#">Kontakt</a>
          </li>
          <li>
            <button className={NavbarCSS.btnLogin}>
              <a className={NavbarCSS.btnLink} href="#">
                Zaloguj się
              </a>
            </button>
          </li>
        </ul>
      </nav>
    </header>
  );
}
