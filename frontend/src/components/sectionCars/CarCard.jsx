/* eslint-disable react/prop-types */
import ButtonMultipurpose from '../shared/ButtonMultipurpose';
import CarAttribute from './CarAttribute';
import styles from './CarCard.module.css';
import images from '/src/images.jsx';
const testImg = 'https://www.topgear.com/sites/default/files/2022/07/6_0.jpg';

const CarCard = ({ car }) => {
	const { brand, carInfo, model, pricePerDay, imagePath } = car;
	// console.log(car);
	return (
		<div className={styles.carCard}>
			<img src={testImg} alt={'essa'}></img>

			<h2>
				{brand} {model}
			</h2>
			<p>{pricePerDay}zł/doba</p>
			<div className={styles.carAttributes}>
				<CarAttribute
					emoji="🐎"
					text={carInfo.mileage / 100 + 'km przebiegu'}
				/>
				<CarAttribute emoji="👍" text={carInfo.gearboxType} />
				<CarAttribute
					emoji="💺"
					text={carInfo.seatsNumber + ' miejsc siedzących'}
				/>
				<CarAttribute emoji="🛢" text={carInfo.fuelType} />
			</div>
			<ButtonMultipurpose to={`/details/${car.id}`}>
				Szczegóły
			</ButtonMultipurpose>
		</div>
	);
};

export default CarCard;
